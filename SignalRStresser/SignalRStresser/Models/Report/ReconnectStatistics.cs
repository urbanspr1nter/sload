using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class ReconnectStatistics
    {
        [JsonProperty("average")]
        public double Average { get; set; }

        [JsonProperty("p50")]
        public double P50 { get; set; }

        [JsonProperty("p90")]
        public double P90 { get; set; }

        [JsonProperty("p95")]
        public double P95 { get; set; }

        [JsonProperty("p99")]
        public double P99 { get; set; }

        [JsonProperty("totalReconnectsInPersistence")]
        public int TotalReconnectsDuringPersistence { get; set; }
    }
}
