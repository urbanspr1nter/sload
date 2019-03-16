using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace TestHub.Hubs
{
    public class TestHub : Hub
    {
        public Task ReceiveRequest(string message)
        {
            var newMessage = "Hello";

            return Clients.Caller.SendAsync("ReceiveResponse", newMessage);
        }

        public Task Ping(string message)
        {
            return Clients.Caller.SendAsync("Pong", message);
        }
    }
}
