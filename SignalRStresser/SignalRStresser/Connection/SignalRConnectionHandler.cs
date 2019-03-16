using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRStresser.Connection
{
    class SignalRConnectionHandler : IConnectionHandler
    {
        private const string _hubMethod = "ReceiveRequest";
        private const string _receiveMethod = "ReceiveResponse";

        private HubConnectionContext _connectionContext;
        private HubConnection _hub;

        private List<long> _resolvedTimes;
        private Stopwatch _watch;
        private string _payload;

        public bool Running
        {
            get => _watch != null && _watch.IsRunning;
        }

        public long ElapsedMilliseconds
        {
            get => _watch.ElapsedMilliseconds;
        }

        public List<long> ResolvedTImes
        {
            get => new List<long>(_resolvedTimes);
        }

        public Task InvocationResult { get; private set; }

        public SignalRConnectionHandler(HubConnection hub, HubConnectionContext connectionContext)
        {
            _hub = hub;
            _connectionContext = connectionContext;
            _resolvedTimes = new List<long>();
            _watch = null;

            var payloadData = new Dictionary<string, string>
            {
                { "thread",  connectionContext.Id }
            };
            _payload = Newtonsoft.Json.JsonConvert.SerializeObject(payloadData);
        }


        public void Reset()
        {
            _watch = null;
            _connectionContext.Reset();
        }

        public void SendRequest()
        {
            _watch = Stopwatch.StartNew();
            InvocationResult = _hub.InvokeAsync(_hubMethod, _payload);
        }

        public void HandleResponse(object message)
        {
            _watch.Stop();

            _resolvedTimes.Add(_watch.ElapsedMilliseconds);
            _resolvedTimes.Sort();
        }

    }
}
