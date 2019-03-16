using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignalRStresser.Models
{
    class RunParameters
    {
        [JsonProperty("isMasterNode")]
        public bool MasterNode { get; set; } = true;

        [JsonProperty("masterNodeHostname")]
        public string MasterNodeHostname { get; set; } = "localhost";

        [JsonProperty("masterNodeListeningPort")]
        public int  MasterNodeListeningPort { get; set; } = 8888;

        [JsonProperty("agentNodeHostnames")]
        public List<string> AgentNodeHostnames { get; set; } = new List<string>();

        [JsonProperty("agentNodeListeningPort")]
        public int AgentNodeListeningPort { get; set; } = 9999;

        [JsonProperty("agentNodePingInterval")]
        public int AgentNodePingInterval { get; set; } = 30000;

        [JsonProperty("maxConnections")]
        public int MaxConnections { get; set; } = 10;

        [JsonProperty("hubUrl")]
        public string HubUrl { get; set; } = "";

        [JsonProperty("outputDirectory")]
        public string OutputDirectory { get; set; } = System.IO.Path.Combine(System.Environment.CurrentDirectory.ToString(), "sload_results\\");

        [JsonProperty("persistConnectionTime")]
        public int PersistConnectionTime { get; set; } = 3;

        [JsonProperty("persistConnectionInterval")]
        public int PersistConnectionInterval { get; set; } = 30000;

        [JsonProperty("persistConnectionPayloadSize")]
        public int PersistConnectionPayloadSize { get; set; } = 2048;

        [JsonProperty("numberOfCpus")]
        public int NumberOfCpus { get; set; } = System.Environment.ProcessorCount;

        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("rampUpInterval")]
        public int RampUpInterval { get; set; } = 1500;

        [JsonProperty("pingInterval")]
        public int PingInterval { get; set; } = 10000;

        [JsonProperty("pingSize")]
        public int PingSize { get; set; } = 1;

        [JsonProperty("keepAliveInterval")]
        public int KeepAliveInterval { get; set; } = 2000;

        [JsonProperty("handshakeTimeout")]
        public int HandShakeTimeout { get; set; } = 60000;

        [JsonProperty("serverTimeout")]
        public int ServerTimeout { get; set; } = 120000;
    }
}
