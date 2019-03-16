using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class CallTimes
    {
        [JsonProperty("workerId")]
        public string WorkerId { get; set; } = "master";

        [JsonProperty("times")]
        public List<long> Times { get; set; } = new List<long>();
    }
}
