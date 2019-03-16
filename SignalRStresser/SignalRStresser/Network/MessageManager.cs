using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using SignalRStresser.Models;
using SignalRStresser.Models.Report;
using SignalRStresser.Remote;
using System.Threading.Tasks;

namespace SignalRStresser.Network
{
    class MessageManager
    {
        private AgentProvider _agentProvider;
        private BenchmarkContext _context;

        public MessageManager(AgentProvider agentProvider, BenchmarkContext context)
        {
            _agentProvider = agentProvider;
            _context = context;
        }

        public void Listen(string hostname, int port)
        {
            var addresses = SocketUtils.GetIpAddresses(hostname);
            foreach (var address in addresses)
            {
                IPEndPoint ipEndpoint = new IPEndPoint(address, port);
                Socket listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    listener.Bind(ipEndpoint);
                    listener.Listen(100);
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    Console.WriteLine($"Node {address.ToString()} is listening on port {port}.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            ClientSocketState state = new ClientSocketState();
            state.WorkSocket = handler;

            handler.BeginReceive(state.Buffer, 0, ClientSocketState.BufferSize, 0, new AsyncCallback(ReadCallback), state);

            listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            ClientSocketState state = (ClientSocketState)ar.AsyncState;
            Socket handler = state.WorkSocket;

            int read = handler.EndReceive(ar);

            if (read > 0)
            {
                state.sb.Append(System.Text.Encoding.ASCII.GetString(state.Buffer, 0, read));
                handler.BeginReceive(state.Buffer, 0, ClientSocketState.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            else
            {
                string content = state.sb.ToString();

                RemoteMessage message = Newtonsoft.Json.JsonConvert.DeserializeObject<RemoteMessage>(content);

                if (message.Type == Remote.Enums.RemoteMessageType.Results)
                {
                    ResultsReport resultsReport = Parser.GetReportData(message);

                    _agentProvider.SetResult(resultsReport.Context.WorkerId, resultsReport);
                }
                else if (message.Type == Remote.Enums.RemoteMessageType.WorkComplete)
                {
                    WorkCompleteMessage workerCompleteMessage = Parser.GetWorkCompleteMessage(message);
                    _agentProvider.SetComplete(workerCompleteMessage.WorkerId);
                }
                else if (message.Type == Remote.Enums.RemoteMessageType.StartWorker)
                {
                    StartWorkerMessage swM = Newtonsoft.Json.JsonConvert.DeserializeObject<StartWorkerMessage>(message.Body);
                    _context.RunParameters = swM.RunParameters;
                    _context.MoveToBenchmarkState(Enums.BenchmarkState.ParametersReceived);
                }
                else if(message.Type == Remote.Enums.RemoteMessageType.Ping)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Ping from master.");

                    // build a pong message.
                    RemoteMessage pongMessage = new RemoteMessage {
                        Type = Remote.Enums.RemoteMessageType.Pong,
                        WorkerId = _context.RunParameters.WorkerId,
                        Body = Newtonsoft.Json.JsonConvert.SerializeObject(new Messages.PongMessage())
                    };

                    SendMessageToNode(pongMessage, _context.RunParameters.MasterNodeHostname, _context.RunParameters.MasterNodeListeningPort);
                }
                else if(message.Type == Remote.Enums.RemoteMessageType.Pong)
                {
                    Console.WriteLine($"Pong from agent: {message.WorkerId}.");
                    _agentProvider.SetLastPingPongTime(message.WorkerId, TimeSpan.FromTicks(DateTime.UtcNow.Ticks));
                }

                handler.Close();
            }
        }

        public void SendMessageToNode(RemoteMessage message, string hostname, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostname);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint ipe = new IPEndPoint(ipAddress, port);

            Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
            s.Connect(ipe);

            SendMessage(message, s);
        }

        public async Task Ping(string workerId, string hostname, int port)
        {
            RemoteMessage message = new RemoteMessage
            {
                Type = Remote.Enums.RemoteMessageType.Ping,
                WorkerId = workerId,
                Body = Newtonsoft.Json.JsonConvert.SerializeObject(new Messages.PingMessage())
            };

            await new TaskFactory().StartNew(() => SendMessageToNode(message, hostname, port));
        }

        private void SendMessage(RemoteMessage message, Socket s)
        {
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            for (int i = 0; i < data.Length; i += ClientSocketState.BufferSize)
            {
                int bytesSent = 0;
                byte[] messageBuffer;
                if (i + ClientSocketState.BufferSize >= data.Length)
                {
                    messageBuffer = Encoding.ASCII.GetBytes(data.Substring(i));
                }
                else
                {
                    messageBuffer = Encoding.ASCII.GetBytes(data.Substring(i, ClientSocketState.BufferSize));
                }

                bytesSent = s.Send(messageBuffer);
            }

            s.Shutdown(SocketShutdown.Send);
            s.Close();
        }
    }
}
