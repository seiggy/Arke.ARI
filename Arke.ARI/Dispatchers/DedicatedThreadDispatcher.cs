using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Dispatchers
{
    public sealed class DedicatedThreadDispatcher : IAriDispatcher
    {
        readonly BlockingCollection<Action> _actionQueue = new();
        readonly CancellationTokenSource _threadCancellation = new();

        public DedicatedThreadDispatcher()
        {
            var cancellationToken = _threadCancellation.Token;
            var queue = _actionQueue;

            var thread = new Thread(() => EventDispatcherThread(cancellationToken, queue));
            thread.Start();
        }

        public void Dispose()
        {
            // No resources to dispose
        }

        public void QueueAction(Action action)
        {
            _actionQueue.Add(action ?? throw new ArgumentNullException(nameof(action)));
        }

        public Task QueueActionAsync(Func<Task> action)
        {
            _actionQueue.Add(() => action?.Invoke());
            return Task.CompletedTask;
        }

        static void EventDispatcherThread(CancellationToken cancellationToken, BlockingCollection<Action> actionQueue)
        {
            try
            {
                while (true)
                {
                    var action = actionQueue.Take(cancellationToken);
                    action();
                }
            }
            catch (OperationCanceledException)
            {
                // Thread is being cancelled, exit gracefully
            }
        }
    }
}
