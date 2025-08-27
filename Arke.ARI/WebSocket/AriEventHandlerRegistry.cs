using System;
using System.Collections.Generic;
using System.Linq;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    public class AriEventHandlerRegistry
    {
        private readonly Dictionary<string, List<IAriEventHandler>> _genericHandlers = new Dictionary<string, List<IAriEventHandler>>();
        private readonly Dictionary<string, List<object>> _typedHandlers = new Dictionary<string, List<object>>();
        private readonly List<IAriEventHandler> _allEventHandlers = new List<IAriEventHandler>();

        public void RegisterHandler(IAriEventHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (_allEventHandlers)
            {
                if (!_allEventHandlers.Contains(handler))
                {
                    _allEventHandlers.Add(handler);
                }
            }
        }

        public void RegisterHandler(string eventType, IAriEventHandler handler)
        {
            if (string.IsNullOrEmpty(eventType)) throw new ArgumentNullException(nameof(eventType));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (_genericHandlers)
            {
                if (!_genericHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<IAriEventHandler>();
                    _genericHandlers[eventType] = handlers;
                }
                if (!handlers.Contains(handler)) handlers.Add(handler);
            }
        }

        public void RegisterHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var eventType = typeof(T).Name;
            lock (_typedHandlers)
            {
                if (!_typedHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers = new List<object>();
                    _typedHandlers[eventType] = handlers;
                }
                if (!handlers.Contains(handler)) handlers.Add(handler);
            }
        }

        public void UnregisterHandler(IAriEventHandler handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (_allEventHandlers)
            {
                _allEventHandlers.Remove(handler);
            }
        }

        public void UnregisterHandler(string eventType, IAriEventHandler handler)
        {
            if (string.IsNullOrEmpty(eventType)) throw new ArgumentNullException(nameof(eventType));
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            lock (_genericHandlers)
            {
                if (_genericHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);
                    if (handlers.Count == 0) _genericHandlers.Remove(eventType);
                }
            }
        }

        public void UnregisterHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            var eventType = typeof(T).Name;
            lock (_typedHandlers)
            {
                if (_typedHandlers.TryGetValue(eventType, out var handlers))
                {
                    handlers.Remove(handler);
                    if (handlers.Count == 0) _typedHandlers.Remove(eventType);
                }
            }
        }

        public void DispatchEvent(Event eventData)
        {
            if (eventData == null) throw new ArgumentNullException(nameof(eventData));
            var eventType = eventData.Type;

            List<IAriEventHandler> allHandlersCopy;
            lock (_allEventHandlers)
            {
                allHandlersCopy = _allEventHandlers.ToList();
            }
            foreach (var handler in allHandlersCopy)
            {
                try { handler.HandleEvent(eventData); } catch { }
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                List<IAriEventHandler> eventHandlersCopy;
                lock (_genericHandlers)
                {
                    eventHandlersCopy = _genericHandlers.TryGetValue(eventType, out var handlers) ? handlers.ToList() : new List<IAriEventHandler>();
                }
                foreach (var handler in eventHandlersCopy)
                {
                    try { handler.HandleEvent(eventData); } catch { }
                }
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                List<object> typedHandlersCopy;
                lock (_typedHandlers)
                {
                    typedHandlersCopy = _typedHandlers.TryGetValue(eventType, out var handlers) ? handlers.ToList() : new List<object>();
                }
                var eventType2 = eventData.GetType();
                var methodInfo = typeof(IAriEventHandler<>).MakeGenericType(eventType2).GetMethod("HandleEvent");
                foreach (var handler in typedHandlersCopy)
                {
                    try { methodInfo?.Invoke(handler, new object[] { eventData }); } catch { }
                }
            }
        }
    }
}
