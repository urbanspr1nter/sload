using System;
using System.Text;
using SignalRStresser.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRStresser.HubCommands
{
    class PingCommand : IHubCommand
    {
        static readonly string alphanum = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        private BenchmarkContext _context;

        public PingCommand(BenchmarkContext context)
        {
            this._context = context;
        }

        public void Receive(string message)
        {
            return;
        }

        public string ReceiveMethodName()
        {
            return "Pong";
        }

        public void Send(HubConnection hub)
        {
            int size = this._context.RunParameters.PingSize;

            if(_context.BenchmarkState == Enums.BenchmarkState.PersistingConnections)
            {
                size = this._context.RunParameters.PersistConnectionPayloadSize;
            }

            Random rand = new Random(DateTime.UtcNow.Millisecond);
            StringBuilder randomMessageBuilder = new StringBuilder();

            for (int i = 0; i < size; i++)
            {
                int pos = rand.Next(0, alphanum.Length);

                randomMessageBuilder.Append(alphanum[pos]);
            }

            hub.InvokeAsync("Ping", randomMessageBuilder.ToString());
        }

        public string SendMethodName()
        {
            return "Ping";
        }
    }
}
