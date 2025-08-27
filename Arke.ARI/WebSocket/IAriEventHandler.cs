using Arke.ARI.Models;

namespace Arke.ARI.WebSocket
{
    public interface IAriEventHandler
    {
        void HandleEvent(Event eventData);
    }

    public interface IAriEventHandler<T> where T : Event
    {
        void HandleEvent(T eventData);
    }
}
