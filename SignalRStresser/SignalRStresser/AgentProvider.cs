using System;
using System.Linq;
using System.Collections.Generic;
using SignalRStresser.Models;
using SignalRStresser.Remote;

namespace SignalRStresser
{
    class AgentProvider
    {
        private Dictionary<string, AgentContext> _workers;
        private BenchmarkContext _context;

        public AgentProvider(BenchmarkContext context)
        {
            _workers = new Dictionary<string, AgentContext>();
            _context = context;
        }

        public StartWorkerMessage StartWorker(string slaveHostname)
        {
            StartWorkerMessage swMessage = new StartWorkerMessage();

            var workerId = Guid.NewGuid().ToString();

            swMessage.RunParameters = new RunParameters
            {
                MasterNode = false,
                MasterNodeHostname = _context.RunParameters.MasterNodeHostname,
                MasterNodeListeningPort = _context.RunParameters.MasterNodeListeningPort,
                AgentNodeHostnames = new List<string> { slaveHostname },
                AgentNodeListeningPort = _context.RunParameters.AgentNodeListeningPort,
                MaxConnections = _context.RunParameters.MaxConnections,
                HubUrl = _context.RunParameters.HubUrl,
                OutputDirectory = _context.RunParameters.OutputDirectory,
                PersistConnectionTime = _context.RunParameters.PersistConnectionTime,
                PersistConnectionInterval = _context.RunParameters.PersistConnectionInterval,
                PersistConnectionPayloadSize = _context.RunParameters.PersistConnectionPayloadSize,
                WorkerId = workerId,
                RampUpInterval = _context.RunParameters.RampUpInterval,
                PingInterval = _context.RunParameters.PingInterval,
                KeepAliveInterval = _context.RunParameters.KeepAliveInterval,
                HandShakeTimeout = _context.RunParameters.HandShakeTimeout,
                ServerTimeout = _context.RunParameters.ServerTimeout
            };

            _workers.Add(workerId, new AgentContext {
                WorkerId = workerId,
                Hostname = slaveHostname,
                Port = 9999,
                UtcSpawnTime = DateTime.UtcNow,
                Status = Enums.AgentStatus.Working
            });

            return swMessage;
        }

        public bool WorkersComplete()
        {
            return _workers.Values.ToList().Find(x => x.Status != Enums.AgentStatus.Completed && x.Status != Enums.AgentStatus.Dead) == null;
        }

        public void SetResult(string workerId, Models.Report.ResultsReport results)
        {
            _workers[workerId].ResultsReport = results;
        }

        public void SetComplete(string workerId)
        {
            _workers[workerId].Status = Enums.AgentStatus.Completed;
        }

        public void SetLastPingPongTime(string workerId, TimeSpan lastPingPong)
        {
            _workers[workerId].LastPingTime = lastPingPong;
        }

        public List<AgentContext> WorkerContexts { get => _workers.Values.ToList();  }
    }
}
