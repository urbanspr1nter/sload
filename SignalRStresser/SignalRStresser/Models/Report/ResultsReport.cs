using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class ResultsReport
    {
        [JsonProperty("context")]
        public BenchmarkContext Context { get; set; }

        [JsonProperty("times")]
        public CallTimes Times { get; set; }

        [JsonProperty("reconnectResults")]
        public List<ConnectionResult> ReconnectResults { get; set; } = new List<ConnectionResult>();

        [JsonProperty("errorLog")]
        public List<Error.ErrorEntry> ErrorLog { get; set; }
    }
}
