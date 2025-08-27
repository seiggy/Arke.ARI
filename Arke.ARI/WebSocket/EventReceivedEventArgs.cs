using System;
using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    public class EventReceivedEventArgs : EventArgs
    {
        public Event EventData { get; }
        public string RawData { get; }

        public EventReceivedEventArgs(Event eventData, string rawData)
        {
            EventData = eventData;
            RawData = rawData;
        }
    }
}
