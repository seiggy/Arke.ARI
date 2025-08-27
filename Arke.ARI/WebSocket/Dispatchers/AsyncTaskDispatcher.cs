using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.WebSocket.Dispatchers
{
    public class AsyncTaskDispatcher : IDispatcher
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxConcurrentTasks;
        private bool _disposed;

        public AsyncTaskDispatcher(int maxConcurrentTasks = 0)
        {
            _maxConcurrentTasks = maxConcurrentTasks > 0 ? maxConcurrentTasks : int.MaxValue;
            _semaphore = maxConcurrentTasks > 0 ? new SemaphoreSlim(maxConcurrentTasks) : null;
        }

        public void Dispatch(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_disposed) throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));
            _ = DispatchInternalAsync(action);
        }

        public async Task DispatchAsync(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_disposed) throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));
            await DispatchInternalAsync(action);
        }

        public async Task DispatchAsync(Func<Task> asyncAction)
        {
            if (asyncAction == null) throw new ArgumentNullException(nameof(asyncAction));
            if (_disposed) throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));

            if (_semaphore != null)
            {
                await _semaphore.WaitAsync();
                try
                {
                    await Task.Run(asyncAction);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                await Task.Run(asyncAction);
            }
        }

        private async Task DispatchInternalAsync(Action action)
        {
            if (_semaphore != null)
            {
                await _semaphore.WaitAsync();
                try
                {
                    await Task.Run(action);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            else
            {
                await Task.Run(action);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _semaphore?.Dispose();
            }
            _disposed = true;
        }
    }
}
