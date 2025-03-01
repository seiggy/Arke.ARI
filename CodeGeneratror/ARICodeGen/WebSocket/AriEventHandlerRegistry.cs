using System;
using System.Collections.Generic;
using System.Linq;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Registry for ARI event handlers.
    /// </summary>
    public class AriEventHandlerRegistry
    {
        private readonly Dictionary<string, List<IAriEventHandler>> _genericHandlers = new Dictionary<string, List<IAriEventHandler>>();
        private readonly Dictionary<string, List<object>> _typedHandlers = new Dictionary<string, List<object>>();
        private readonly List<IAriEventHandler> _allEventHandlers = new List<IAriEventHandler>();

        /// <summary>
        /// Registers a handler for all events.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        public void RegisterHandler(IAriEventHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_allEventHandlers)
            {
                if (!_allEventHandlers.Contains(handler))
                {
                    _allEventHandlers.Add(handler);
                }
            }
        }

        /// <summary>
        /// Registers a handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="handler">The handler to register.</param>
        public void RegisterHandler(string eventType, IAriEventHandler handler)
        {
            if (string.IsNullOrEmpty(eventType))
                throw new ArgumentNullException(nameof(eventType));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_genericHandlers)
            {
                if (!_genericHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<IAriEventHandler>();
                    _genericHandlers[eventType] = handlers;
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                }
            }
        }

        /// <summary>
        /// Registers a typed handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The handler to register.</param>
        public void RegisterHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T).Name;
            
            lock (_typedHandlers)
            {
                if (!_typedHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<object>();
                    _typedHandlers[eventType] = handlers;
                }

                if (!handlers.Contains(handler))
                {
                    handlers.Add(handler);
                }
            }
        }

        /// <summary>
        /// Unregisters a handler for all events.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        public void UnregisterHandler(IAriEventHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_allEventHandlers)
            {
                _allEventHandlers.Remove(handler);
            }
        }

        /// <summary>
        /// Unregisters a handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="handler">The handler to unregister.</param>
        public void UnregisterHandler(string eventType, IAriEventHandler handler)
        {
            if (string.IsNullOrEmpty(eventType))
                throw new ArgumentNullException(nameof(eventType));
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_genericHandlers)
            {
                if (_genericHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);
                    if (handlers.Count == 0)
                    {
                        _genericHandlers.Remove(eventType);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters a typed handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The handler to unregister.</param>
        public void UnregisterHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T).Name;
            
            lock (_typedHandlers)
            {
                if (_typedHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);
                    if (handlers.Count == 0)
                    {
                        _typedHandlers.Remove(eventType);
                    }
                }
            }
        }

        /// <summary>
        /// Dispatches an event to all registered handlers.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        public void DispatchEvent(Event eventData)
        {
            if (eventData == null)
                throw new ArgumentNullException(nameof(eventData));

            var eventType = eventData.Type;

            // Dispatch to all event handlers
            List<IAriEventHandler> allHandlersCopy;
            lock (_allEventHandlers)
            {
                allHandlersCopy = _allEventHandlers.ToList();
            }

            foreach (var handler in allHandlersCopy)
            {
                try
                {
                    handler.HandleEvent(eventData);
                }
                catch (Exception)
                {
                    // Swallow exceptions to prevent one handler from breaking others
                    // In a production environment, you would log this exception
                }
            }

            // Dispatch to event-specific handlers
            if (!string.IsNullOrEmpty(eventType))
            {
                List<IAriEventHandler> eventHandlersCopy;
                lock (_genericHandlers)
                {
                    if (_genericHandlers.TryGetValue(eventType, out var handlers))
                    {
                        eventHandlersCopy = handlers.ToList();
                    }
                    else
                    {
                        eventHandlersCopy = new List<IAriEventHandler>();
                    }
                }

                foreach (var handler in eventHandlersCopy)
                {
                    try
                    {
                        handler.HandleEvent(eventData);
                    }
                    catch (Exception)
                    {
                        // Swallow exceptions to prevent one handler from breaking others
                        // In a production environment, you would log this exception
                    }
                }
            }

            // Dispatch to typed handlers
            if (!string.IsNullOrEmpty(eventType))
            {
                List<object> typedHandlersCopy;
                lock (_typedHandlers)
                {
                    if (_typedHandlers.TryGetValue(eventType, out var handlers))
                    {
                        typedHandlersCopy = handlers.ToList();
                    }
                    else
                    {
                        typedHandlersCopy = new List<object>();
                    }
                }

                // Use reflection to call the typed handlers
                var eventType2 = eventData.GetType();
                var methodInfo = typeof(IAriEventHandler<>).MakeGenericType(eventType2).GetMethod("HandleEvent");

                foreach (var handler in typedHandlersCopy)
                {
                    try
                    {
                        methodInfo?.Invoke(handler, new object[] { eventData });
                    }
                    catch (Exception)
                    {
                        // Swallow exceptions to prevent one handler from breaking others
                        // In a production environment, you would log this exception
                    }
                }
            }
        }
    }
} 