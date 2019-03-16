using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRStresser.HubCommands
{
    interface IHubCommand
    {
        void Send(HubConnection hub);
        void Receive(string message);
        string SendMethodName();
        string ReceiveMethodName();
    }
}
