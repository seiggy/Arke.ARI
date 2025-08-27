using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Dispatchers
{
    public sealed class ThreadPoolDispatcher : IAriDispatcher
    {
        public void Dispose()
        {
            // No resources to dispose
        }

        public void QueueAction(Action action)
        {
            ThreadPool.QueueUserWorkItem(_ => action());
        }

        public Task QueueActionAsync(Func<Task> action)
        {
            ThreadPool.QueueUserWorkItem(async _ => await action());
            return Task.CompletedTask;
        }
    }
}
