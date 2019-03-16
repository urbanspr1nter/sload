using System;
using Newtonsoft.Json;

namespace SignalRStresser.Models.Error
{
    class ConnectionErrorEntry
    {
        [JsonProperty("retryNumber")]
        public int RetryNumber { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("innerMessage")]
        public string InnerMessage { get; set; }

        [JsonProperty("startUtcDate")]
        public DateTime StartUtcDate { get; set; }

        [JsonProperty("dateUtc")]
        public DateTime UtcDate { get; set; } = DateTime.UtcNow;
    }
}
