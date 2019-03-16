using System.ComponentModel;

namespace SignalRStresser.Enums
{
    enum HubConnectionErrorType
    {
        [Description("ConnectionError")]
        ConnectionError = 0,

        [Description("PerformanceError")]
        PerformanceError = 1
    }
}
