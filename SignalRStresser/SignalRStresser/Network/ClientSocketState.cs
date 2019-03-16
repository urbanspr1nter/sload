using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace SignalRStresser.Network
{
    class ClientSocketState
    {
        public Socket WorkSocket = null;
        public const int BufferSize = 4096;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
