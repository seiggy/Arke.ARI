using System;
using System.Threading.Tasks;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Interface for WebSocket event producers.
    /// </summary>
    public interface IEventProducer
    {
        /// <summary>
        /// Event raised when a WebSocket event is received.
        /// </summary>
        event EventHandler<EventReceivedEventArgs> OnEvent;

        /// <summary>
        /// Event raised when the WebSocket connection state changes.
        /// </summary>
        event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;

        /// <summary>
        /// Gets a value indicating whether the WebSocket is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects to the WebSocket.
        /// </summary>
        /// <param name="url">The WebSocket URL.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ConnectAsync(string url);

        /// <summary>
        /// Disconnects from the WebSocket.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DisconnectAsync();
    }
} 