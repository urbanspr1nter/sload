using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Error
{
    class ErrorEntry
    {
        [JsonProperty("workerId")]
        public string WorkerId { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; } = "";

        [JsonProperty("connectionErrors")]
        public List<ConnectionErrorEntry> ConnectionErrors { get; set; } = new List<ConnectionErrorEntry>();

        [JsonProperty("performanceErrors")]
        public List<PerformanceErrorEntry> PerformanceErrors { get; set; } = new List<PerformanceErrorEntry>();

        [JsonProperty("invocationResult")]
        public string Result { get; set; } = "";
    }
}
