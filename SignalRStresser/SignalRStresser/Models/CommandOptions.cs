using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;

namespace SignalRStresser.Models
{
    class CommandOptions
    {
        public CommandOption MasterNode { get; set; }
        public CommandOption MasterNodeHostname { get; set; }
        public CommandOption MasterNodeListeningPort { get; set; }
        public CommandOption AgentListeningHostnames { get; set; }
        public CommandOption AgentListeningPort { get; set; }
        public CommandOption AgentPingInterval { get; set; }
        public CommandOption MaxConnections { get; set; }
        public CommandOption HubUrl { get; set; }
        public CommandOption OutputDirectory { get; set; }
        public CommandOption PersistConnectionTime { get; set; }
        public CommandOption PersistConnectionsInterval { get; set; }
        public CommandOption PersistConnectionsPayloadSize { get; set; }
        public CommandOption WorkerId { get; set; }
        public CommandOption RampUpInterval { get; set; }
        public CommandOption PingInterval { get; set; }
        public CommandOption PingSize { get; set; }
        public CommandOption KeepAliveInterval { get; set; }
        public CommandOption HandShakeTimeout { get; set; }
        public CommandOption ServerTimeout { get; set; }
    }
}
