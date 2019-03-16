using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Remote
{
    class WorkCompleteMessage
    {
        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }
    }
}
