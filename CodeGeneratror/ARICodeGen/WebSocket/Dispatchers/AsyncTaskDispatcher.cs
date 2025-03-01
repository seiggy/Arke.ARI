using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.WebSocket.Dispatchers
{
    /// <summary>
    /// Dispatcher that executes actions as asynchronous tasks.
    /// </summary>
    /// <remarks>
    /// This dispatcher uses Task.Run to execute actions, but unlike the ThreadPoolDispatcher,
    /// it supports awaiting task completion and proper propagation of exceptions.
    /// </remarks>
    public class AsyncTaskDispatcher : IDispatcher
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _maxConcurrentTasks;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTaskDispatcher"/> class.
        /// </summary>
        /// <param name="maxConcurrentTasks">The maximum number of concurrent tasks. Default is unlimited.</param>
        public AsyncTaskDispatcher(int maxConcurrentTasks = 0)
        {
            _maxConcurrentTasks = maxConcurrentTasks > 0 ? maxConcurrentTasks : int.MaxValue;
            _semaphore = maxConcurrentTasks > 0 ? new SemaphoreSlim(maxConcurrentTasks) : null;
        }

        /// <summary>
        /// Dispatches an action to be executed as an asynchronous task.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        public void Dispatch(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (_disposed)
                throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));

            // Fire and forget
            _ = DispatchInternalAsync(action);
        }

        /// <summary>
        /// Dispatches a synchronous action to be executed as an asynchronous task.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        /// <returns>A task that completes when the action has been executed.</returns>
        public async Task DispatchAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (_disposed)
                throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));

            await DispatchInternalAsync(action);
        }

        /// <summary>
        /// Dispatches an asynchronous action to be executed.
        /// </summary>
        /// <param name="asyncAction">The asynchronous action to dispatch.</param>
        /// <returns>A task that completes when the asynchronous action has completed.</returns>
        public async Task DispatchAsync(Func<Task> asyncAction)
        {
            if (asyncAction == null)
                throw new ArgumentNullException(nameof(asyncAction));

            if (_disposed)
                throw new ObjectDisposedException(nameof(AsyncTaskDispatcher));

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

        /// <summary>
        /// Disposes the dispatcher.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the dispatcher.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _semaphore?.Dispose();
            }

            _disposed = true;
        }
    }
} 