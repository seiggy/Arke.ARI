using System;
using System.Threading.Tasks;

namespace Arke.ARI.WebSocket.Dispatchers
{
    public interface IDispatcher : IDisposable
    {
        Task DispatchAsync(Action action);
        Task DispatchAsync(Func<Task> asyncAction);
        void Dispatch(Action action);
    }
}
