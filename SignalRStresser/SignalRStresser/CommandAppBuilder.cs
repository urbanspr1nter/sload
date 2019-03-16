using SignalRStresser.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace SignalRStresser
{
    class CommandAppBuilder
    {
        public CommandOptions Build(BenchmarkContext context, CommandLineApplication cmdApp)
        {
            cmdApp.Name = "sload";
            cmdApp.Description = "A SignalR Core load tester";
            cmdApp.HelpOption("-h|--help");

            var masterNodeOption = cmdApp.Option("--isMasterNode", "Is this a master node?", CommandOptionType.SingleValue);
            var masterNodeHostnameOption = cmdApp.Option("--masterNodeHostname", "Hostname of the designated master node. Default: localhost", CommandOptionType.SingleValue);
            var masterNodeListeningPortOption = cmdApp.Option("--masterNodeListeningPort", "Port of the master node. Default: 8888", CommandOptionType.SingleValue);
            var agentHostnamesOption = cmdApp.Option("--agentNodeHostnames", "The list of hostnames for the agents.", CommandOptionType.SingleValue);
            var agentListeningPortOption = cmdApp.Option("--agentNodeListeningPort", "Port of the agent to listen on. Default: 8889", CommandOptionType.SingleValue);
            var maxConnectionOption = cmdApp.Option("--maxConnections", "The total connections to connect to the SignalR service. Default: 10", CommandOptionType.SingleValue);
            var hubUrlOption = cmdApp.Option("--hubUrl", "The hub URL to connect to. Required.", CommandOptionType.SingleValue);
            var outputDirectoryOption = cmdApp.Option("--outputDirectory", "The output directory to write the results to. Default: The current working directory.", CommandOptionType.SingleValue);
            var persistConnectionsTimeOption = cmdApp.Option("--persistConnectionTime", "Duration of time (s) to persist the connections. Default: 3", CommandOptionType.SingleValue);
            var persistConnectionsIntervalOption = cmdApp.Option("--persistConnectionInterval", "Interval duration (ms) to keep sending messages to open connections. Default: 30000", CommandOptionType.SingleValue);
            var persistConnectionsPayloadSize = cmdApp.Option("--persistConnectionPayloadSize", "Bytes for each connection to send during persistence. Default: 2048", CommandOptionType.SingleValue);
            var workerIdOption = cmdApp.Option("--workerId", "The worker ID. Default: 0", CommandOptionType.SingleValue);
            var rampUpIntervalOption = cmdApp.Option("--rampUpInterval", "Interval duration to ramp up connections. Default: 1500", CommandOptionType.SingleValue);
            var pingIntervalOption = cmdApp.Option("--pingInterval", "Interval duration (ms) to ping the connection for activity. Default: 10000.", CommandOptionType.SingleValue);
            var pingSizeOption = cmdApp.Option("--pingSize", "The number of bytes to send on Ping. Default: 1", CommandOptionType.SingleValue);
            var keepAliveIntervalOption = cmdApp.Option("--keepAliveInterval", "Interval duration (ms) to maintain the TCP connection. Default: 2000.", CommandOptionType.SingleValue);
            var handshakeTimeoutOption = cmdApp.Option("--handshakeTimeout", "Duration (ms) before a timeout for handshake. Default: 60000.", CommandOptionType.SingleValue);
            var serverTimeoutOption = cmdApp.Option("--serverTimeout", "Duration (ms) before timing out from not hearing back from server. Default: 120000", CommandOptionType.SingleValue);
            var agentNodePingIntervalOption = cmdApp.Option("--agentNodePingInterval", "Duration (ms) of ping-pong operation to and from workers. Default: 30000.", CommandOptionType.SingleValue);

            return new CommandOptions
            {
                MasterNode = masterNodeOption,
                MasterNodeHostname = masterNodeHostnameOption,
                MasterNodeListeningPort = masterNodeListeningPortOption,
                AgentListeningHostnames = agentHostnamesOption,
                AgentListeningPort = agentListeningPortOption,
                AgentPingInterval = agentNodePingIntervalOption,
                MaxConnections = maxConnectionOption,
                HubUrl = hubUrlOption,
                OutputDirectory = outputDirectoryOption,
                PersistConnectionTime = persistConnectionsTimeOption,
                PersistConnectionsInterval = persistConnectionsIntervalOption,
                PersistConnectionsPayloadSize = persistConnectionsPayloadSize,
                WorkerId = workerIdOption,
                RampUpInterval = rampUpIntervalOption,
                PingInterval = pingIntervalOption,
                PingSize = pingSizeOption,
                KeepAliveInterval = keepAliveIntervalOption,
                HandShakeTimeout = handshakeTimeoutOption,
                ServerTimeout = serverTimeoutOption
            };
        }
    }
}
