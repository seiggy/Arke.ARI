using System;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Event arguments for WebSocket message events.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the message received from the WebSocket.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The received message.</param>
        public MessageEventArgs(string message)
        {
            Message = message;
        }
    }
} 