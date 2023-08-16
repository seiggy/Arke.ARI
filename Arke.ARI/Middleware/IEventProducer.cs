using Arke.ARI.Middleware.Default;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using static Arke.ARI.Middleware.Default.WebSocketEventProducer;

namespace Arke.ARI.Middleware
{
    public enum ConnectionState
    {
        None = WebSocketState.None,
        Connecting = WebSocketState.Connecting,
        Open = WebSocketState.Open,
        Closing = WebSocketState.CloseReceived | WebSocketState.CloseSent,
        Closed = WebSocketState.Closed
    }

    public class MessageEventArgs
    {
        public string Message;
    }

    public delegate Task MessageReceivedHandler(IEventProducer sender, string message);
    public delegate Task ConnectionStateChangedHandler(IEventProducer sender);

    public interface IEventProducer
    {
        ConnectionState State { get; }
        event MessageReceivedHandler OnMessageReceived;
        event ConnectionStateChangedHandler OnConnectionStateChanged;

        Task ConnectAsync(bool subscribeAll = false, bool ssl = false);
        Task DisconnectAsync();
    }
}