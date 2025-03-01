using System;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Interface for handling ARI events.
    /// </summary>
    public interface IAriEventHandler
    {
        /// <summary>
        /// Handles an ARI event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        void HandleEvent(Event eventData);
    }

    /// <summary>
    /// Interface for handling a specific type of ARI event.
    /// </summary>
    /// <typeparam name="T">The type of event to handle.</typeparam>
    public interface IAriEventHandler<T> where T : Event
    {
        /// <summary>
        /// Handles a specific type of ARI event.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        void HandleEvent(T eventData);
    }
} 