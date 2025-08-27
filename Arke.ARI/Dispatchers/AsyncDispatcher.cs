using System;
using System.Threading.Tasks;

namespace Arke.ARI.Dispatchers
{
    public sealed class AsyncDispatcher : IAriDispatcher
    {
        public void Dispose()
        {

        }

        public async void QueueAction(Action action)
        {
            await Task.Run(action);
        }

        public async Task QueueActionAsync(Func<Task> action)
        {
            await Task.Run(action);
        }
    }
}
