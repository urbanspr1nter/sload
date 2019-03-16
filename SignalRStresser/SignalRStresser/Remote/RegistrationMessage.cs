using Newtonsoft.Json;

namespace SignalRStresser.Remote
{
    class RegistrationMessage
    {
        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("ipAddress")]
        public long IpAddress { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
    }
}
