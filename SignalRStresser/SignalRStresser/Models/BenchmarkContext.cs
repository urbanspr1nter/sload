using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SignalRStresser.Enums;

namespace SignalRStresser.Models
{
    class BenchmarkContext
    {
        [JsonProperty("masterNode")]
        public bool MasterNode { get; set; }

        [JsonProperty("benchmarkState")]
        public BenchmarkState BenchmarkState { get; set; } = BenchmarkState.Listening;

        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("successfulConnections")]
        public int SuccessfulConnections = 0;

        [JsonProperty("faultedConnections")]
        public int FaultedConnections = 0;

        [JsonProperty("disconnectedConnections")]
        public int DisconnectedConnections = 0;

        [JsonProperty("runParameters")]
        public RunParameters RunParameters { get; set; } = new RunParameters();

        public void MoveToBenchmarkState(BenchmarkState state)
        {
            if(state == BenchmarkState.Listening)
            {
                SuccessfulConnections = 0;
                FaultedConnections = 0;
                DisconnectedConnections = 0;
                RunParameters = new RunParameters();
            }

            BenchmarkState = state;
        }

        public void InitializeBenchmarkContext(CommandOptions options)
        {
            if (!options.HubUrl.HasValue() && this.MasterNode)
            {
                throw new Exception("Required URL");
            }

            if (options.MasterNode.HasValue())
            {
                if (options.MasterNode.Value().Equals("true"))
                {
                    RunParameters.MasterNode = true;
                }
                else
                {
                    RunParameters.MasterNode = false;
                }
            }
            MasterNode = RunParameters.MasterNode;

            if (options.MasterNodeHostname.HasValue())
            {
                RunParameters.MasterNodeHostname = options.MasterNodeHostname.Value();
            }

            if (options.MasterNodeListeningPort.HasValue())
            {
                RunParameters.MasterNodeListeningPort = Convert.ToInt32(options.MasterNodeListeningPort.Value());
            }

            if(options.AgentListeningHostnames.HasValue())
            {
                RunParameters.AgentNodeHostnames = new List<string>(options.AgentListeningHostnames.Value().Split(","));
            }

            if(options.AgentListeningPort.HasValue())
            {
                RunParameters.AgentNodeListeningPort = Convert.ToInt32(options.AgentListeningPort.Value());
            }

            if(options.AgentPingInterval.HasValue())
            {
                RunParameters.AgentNodePingInterval = Convert.ToInt32(options.AgentPingInterval.Value());
            }

            if (options.MaxConnections.HasValue())
            {
                RunParameters.MaxConnections = Convert.ToInt32(options.MaxConnections.Value());
            }

            if (options.HubUrl.HasValue())
            {
                RunParameters.HubUrl = Convert.ToString(options.HubUrl.Value());
            }

            if (options.OutputDirectory.HasValue())
            {
                RunParameters.OutputDirectory = Convert.ToString(options.OutputDirectory.Value());
            }

            if (options.PersistConnectionTime.HasValue())
            {
                RunParameters.PersistConnectionTime = Convert.ToInt32(options.PersistConnectionTime.Value());
            }

            if (options.PersistConnectionsInterval.HasValue())
            {
                RunParameters.PersistConnectionInterval = Convert.ToInt32(options.PersistConnectionsInterval.Value());
            }

            if (options.PersistConnectionsPayloadSize.HasValue())
            {
                RunParameters.PersistConnectionPayloadSize = Convert.ToInt32(options.PersistConnectionsPayloadSize.Value());
            }

            if (options.WorkerId.HasValue())
            {
                RunParameters.WorkerId = options.WorkerId.Value();
            }
            WorkerId = RunParameters.WorkerId;

            if (options.RampUpInterval.HasValue())
            {
               RunParameters.RampUpInterval = Convert.ToInt32(options.RampUpInterval.Value());
            }

            if (options.PingInterval.HasValue())
            {
                RunParameters.PingInterval = Convert.ToInt32(options.PingInterval.Value());
            }

            if (options.PingSize.HasValue())
            {
                RunParameters.PingSize = Convert.ToInt32(options.PingSize.Value());
            }

            if (options.KeepAliveInterval.HasValue())
            {
                RunParameters.KeepAliveInterval = Convert.ToInt32(options.KeepAliveInterval.Value());
            }

            if (options.HandShakeTimeout.HasValue())
            {
                RunParameters.HandShakeTimeout = Convert.ToInt32(options.HandShakeTimeout.Value());
            }

            if (options.ServerTimeout.HasValue())
            {
                RunParameters.ServerTimeout = Convert.ToInt32(options.ServerTimeout.Value());
            }
        }
    }
}
