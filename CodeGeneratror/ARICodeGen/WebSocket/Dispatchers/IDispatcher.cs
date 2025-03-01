using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.WebSocket.Dispatchers
{
    /// <summary>
    /// Interface for dispatchers that execute actions.
    /// </summary>
    public interface IDispatcher : IDisposable
    {
        /// <summary>
        /// Dispatches a synchronous action to be executed.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        /// <returns>A task that completes when the action has been dispatched.</returns>
        Task DispatchAsync(Action action);
        
        /// <summary>
        /// Dispatches an asynchronous action to be executed.
        /// </summary>
        /// <param name="asyncAction">The asynchronous action to dispatch.</param>
        /// <returns>A task that completes when the asynchronous action has been dispatched and completed.</returns>
        Task DispatchAsync(Func<Task> asyncAction);
        
        /// <summary>
        /// Dispatches a synchronous action to be executed.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        /// <remarks>
        /// This method is provided for backward compatibility. New code should use DispatchAsync.
        /// </remarks>
        void Dispatch(Action action);
    }
} 