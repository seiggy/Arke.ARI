namespace Arke.ARI.Models
{
    /// <summary>
    /// Represents the state of the WebSocket connection to the Asterisk ARI server
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// The connection is closed
        /// </summary>
        Closed,
        
        /// <summary>
        /// The connection is in the process of connecting
        /// </summary>
        Connecting,
        
        /// <summary>
        /// The connection is open and ready for communication
        /// </summary>
        Open,
        
        /// <summary>
        /// The connection is closing
        /// </summary>
        Closing,
        
        /// <summary>
        /// The connection attempt failed
        /// </summary>
        Failed
    }
} 