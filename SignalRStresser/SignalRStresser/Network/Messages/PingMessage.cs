using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Network.Messages
{
    class PingMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "ping";
    }
}
