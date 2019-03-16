using System.ComponentModel;

namespace SignalRStresser.Enums
{
    enum BenchmarkState
    {
        [Description("Listening")]
        Listening,

        [Description("ParametersReceived")]
        ParametersReceived,

        [Description("Initialized")]
        Initialized,

        [Description("BuildingConnections")]
        BuildingConnections,

        [Description("TestsPending")]
        TestsPending,

        [Description("TestsRunning")]
        TestsRunning,

        [Description("TestsCompleted")]
        TestsCompleted,

        [Description("PersistingConnections")]
        PersistingConnections,

        [Description("Completed")]
        Completed
    }
}
