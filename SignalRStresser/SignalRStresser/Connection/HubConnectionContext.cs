using System;
using Newtonsoft.Json;
using SignalRStresser.Enums;

namespace SignalRStresser.Connection
{
    class HubConnectionContext
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("connectionInitialized")]
        public bool ConnectionInitialized { get; set; } = false;

        [JsonProperty("currentRetries")]
        public int CurrentRetries { get; set; } = 1;

        [JsonProperty("nextRetryTime")]
        public TimeSpan NextRetry { get; set; } = TimeSpan.FromTicks(DateTime.MinValue.Ticks);

        [JsonProperty("nextTestTime")]
        public TimeSpan NextTest { get; set; } = TimeSpan.FromTicks(DateTime.MinValue.Ticks);

        [JsonProperty("resolved")]
        public bool Resolved { get; set; } = false;

        [JsonProperty("resolvedTime")]
        public long ResolvedTime { get; set; } = -1;

        [JsonProperty("errored")]
        public bool Errored { get; set; } = false;

        [JsonProperty("errorType")]
        public HubConnectionErrorType ErrorType { get; set; }

        [JsonProperty("lastPingTime")]
        public TimeSpan LastPingTime { get; set; } = TimeSpan.FromTicks(DateTime.MinValue.Ticks);

        [JsonProperty("lastPingForPersistTime")]
        public TimeSpan LastPingForPersistTime { get; set; } = TimeSpan.FromTicks(DateTime.MinValue.Ticks);

        [JsonProperty("utcStartDate")]
        public DateTime UtcStartDate { get; set; } = DateTime.UtcNow;

        public void Reset()
        {
            this.CurrentRetries = 1;
            this.NextRetry = TimeSpan.FromTicks(DateTime.MinValue.Ticks);
            this.Resolved = false;
            this.ResolvedTime = -1;
            this.Errored = false;
            this.LastPingTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
            this.LastPingForPersistTime = TimeSpan.FromTicks(DateTime.UtcNow.Ticks);
        }
    }
}
