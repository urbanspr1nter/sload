using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Error
{
    class PerformanceErrorEntry
    {
        [JsonProperty("elapsedMs")]
        public long ElapsedMs { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("dateUtcStart")]
        public DateTime DateUtcStart { get; set; }

        [JsonProperty("dateUtcEnd")]
        public DateTime DateUtcEnd { get; set; }
    }
}
