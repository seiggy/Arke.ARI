using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Dispatchers
{
    sealed class ThreadPoolDispatcher : IAriDispatcher
    {
        public void Dispose()
        {
        }

        public Task QueueAction(Action action)
        {
            ThreadPool.QueueUserWorkItem(_ => action());
            return Task.CompletedTask;
        }
    }
}
