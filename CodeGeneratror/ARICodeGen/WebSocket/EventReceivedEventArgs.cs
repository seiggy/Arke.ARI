using System;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Event arguments for when an event is received from the WebSocket.
    /// </summary>
    public class EventReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the event data.
        /// </summary>
        public Event EventData { get; }

        /// <summary>
        /// Gets the raw JSON data.
        /// </summary>
        public string RawData { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="rawData">The raw JSON data.</param>
        public EventReceivedEventArgs(Event eventData, string rawData)
        {
            EventData = eventData;
            RawData = rawData;
        }
    }
} 