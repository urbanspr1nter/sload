using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class CallerLatencyStatistics
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

        [JsonProperty("totalCalls")]
        public double TotalCalls { get; set; }

        [JsonProperty("fast")]
        public double Time10 { get; set; }

        [JsonProperty("acceptable")]
        public double Time100 { get; set; }

        [JsonProperty("slow")]
        public double Time200 { get; set; }

        [JsonProperty("xslow")]
        public double Time1000 { get; set; }
    }
}
