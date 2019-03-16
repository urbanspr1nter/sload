using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SignalRStresser.Models;

namespace SignalRStresser.Remote
{
    class StartWorkerMessage
    {
        [JsonProperty("runParameters")]
        public RunParameters RunParameters { get; set; }
    }
}
