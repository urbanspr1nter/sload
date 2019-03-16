using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Network.Messages
{
    class PongMessage
    {
        [JsonProperty("content")]
        public string Content { get; set; } = "pong";
    }
}
