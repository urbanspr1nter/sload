using System;
using System.Collections.Generic;
using System.Text;

namespace SignalRStresser.Connection
{
    interface IConnectionHandler
    {
        void Reset();
        void SendRequest();
        void HandleResponse(object message);
    }
}
