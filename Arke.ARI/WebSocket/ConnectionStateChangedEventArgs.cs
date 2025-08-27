using System;

namespace Arke.ARI.WebSocket
{
    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionState PreviousState { get; }
        public ConnectionState NewState { get; }
        public Exception? Exception { get; }

        public ConnectionStateChangedEventArgs(ConnectionState previousState, ConnectionState newState, Exception? exception = null)
        {
            PreviousState = previousState;
            NewState = newState;
            Exception = exception;
        }
    }
}
