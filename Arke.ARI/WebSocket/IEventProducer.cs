using System;
using System.Threading.Tasks;

namespace Arke.ARI.WebSocket
{
    public interface IEventProducer
    {
        event EventHandler<EventReceivedEventArgs> OnEvent;
        event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;
        bool IsConnected { get; }
        Task ConnectAsync(string url);
        Task DisconnectAsync();
    }
}
