using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using SignalRStresser.Models.Report;
using SignalRStresser.Enums;

namespace SignalRStresser.Models
{
    class AgentContext
    {
        public string WorkerId { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public AgentStatus Status { get; set; }
        public DateTime UtcSpawnTime { get; set; }
        public TimeSpan LastPingTime { get; set; } = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
        public ResultsReport ResultsReport { get; set; }
    }
}
