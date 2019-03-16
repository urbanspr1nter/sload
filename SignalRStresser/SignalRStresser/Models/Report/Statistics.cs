using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Report
{
    class Statistics
    {
        [JsonProperty("successfulConnections")]
        public long SuccessfulConnections { get; set; }

        [JsonProperty("disconnectedConnections")]
        public long DisconnectedConnections { get; set; }

        [JsonProperty("faultedConnections")]
        public long FaultedConnections { get; set; }

        [JsonProperty("callerLatencyStats")]
        public CallerLatencyStatistics CallerLatencyStats { get; set; } = new CallerLatencyStatistics();

        [JsonProperty("reconnectStatistics")]
        public ReconnectStatistics ReconnectStats { get; set; } = new ReconnectStatistics();
    }
}
