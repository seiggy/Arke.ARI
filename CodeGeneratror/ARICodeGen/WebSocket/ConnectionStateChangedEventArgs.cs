using System;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Connection state enumeration.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The connection is closed.
        /// </summary>
        Closed,

        /// <summary>
        /// The connection is connecting.
        /// </summary>
        Connecting,

        /// <summary>
        /// The connection is open.
        /// </summary>
        Open,

        /// <summary>
        /// The connection is closing.
        /// </summary>
        Closing,

        /// <summary>
        /// The connection has failed.
        /// </summary>
        Failed
    }

    /// <summary>
    /// Event arguments for connection state changes.
    /// </summary>
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the previous connection state.
        /// </summary>
        public ConnectionState PreviousState { get; }

        /// <summary>
        /// Gets the new connection state.
        /// </summary>
        public ConnectionState NewState { get; }

        /// <summary>
        /// Gets the exception that caused the state change, if any.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionStateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="previousState">The previous connection state.</param>
        /// <param name="newState">The new connection state.</param>
        /// <param name="exception">The exception that caused the state change, if any.</param>
        public ConnectionStateChangedEventArgs(ConnectionState previousState, ConnectionState newState, Exception exception = null)
        {
            PreviousState = previousState;
            NewState = newState;
            Exception = exception;
        }
    }
} 