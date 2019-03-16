using System.Collections.Generic;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class AggregatedResultsReport
    {
        [JsonProperty("runParameters")]
        public List<BenchmarkContext> RunParameters { get; set; } = new List<BenchmarkContext>();

        [JsonProperty("deadWorkers")]
        public List<AgentContext> DeadWorkers { get; set; } = new List<AgentContext>();

        [JsonProperty("errorLogs")]
        public List<List<Error.ErrorEntry>> ErrorLog { get; set; } = new List<List<Error.ErrorEntry>>();

        [JsonProperty("callTimes")]
        public List<CallTimes> CallTimes { get; set; } = new List<CallTimes>();

        [JsonProperty("callStatistics")]
        public Statistics CallStatistics { get; set; } = new Statistics();

        [JsonProperty("reconnectResults")]
        public List<ConnectionResult> ReconnectionResult { get; set; } = new List<ConnectionResult>();
    }
}
