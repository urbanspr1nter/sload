using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SignalRStresser.Network
{
    class SocketUtils
    {
        public static List<IPAddress> GetIpAddresses(string hostname)
        {
            IPHostEntry hostInfo = Dns.GetHostEntry(hostname);

            return new List<IPAddress>(hostInfo.AddressList);
        }
    }
}
