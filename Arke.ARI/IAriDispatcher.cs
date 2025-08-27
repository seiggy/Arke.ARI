using System;
using System.Threading.Tasks;

namespace Arke.ARI
{
    interface IAriDispatcher : IDisposable
    {
        void QueueAction(Action action);
        Task QueueActionAsync(Func<Task> action);
    }
}
