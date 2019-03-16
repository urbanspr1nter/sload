using System;
using System.Collections.Generic;
using System.Text;
using SignalRStresser.Models.Report;

namespace SignalRStresser.Remote
{
    class Parser
    {
        public static ResultsReport GetReportData(RemoteMessage message)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ResultsReport>(message.Body);
        }

        public static RegistrationMessage GetRegistrationMessage(RemoteMessage message)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<RegistrationMessage>(message.Body);
        }

        public static WorkCompleteMessage GetWorkCompleteMessage(RemoteMessage message)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<WorkCompleteMessage>(message.Body);
        }
    }
}
