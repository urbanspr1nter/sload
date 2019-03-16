using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SignalRStresser.Remote.Enums;

namespace SignalRStresser.Remote
{
    class RemoteMessage
    {
        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("type")]
        public RemoteMessageType Type { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
