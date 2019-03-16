using System;
using Newtonsoft.Json;

namespace SignalRStresser.Models
{
    class ConnectionResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("connectedDate")]
        public DateTime UtcConnectedDate { get; set; }
    }
}
