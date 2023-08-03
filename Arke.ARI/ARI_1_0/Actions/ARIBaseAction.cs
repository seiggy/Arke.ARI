using System.Threading;
using System.Threading.Tasks;
using Arke.ARI.Middleware;

namespace Arke.ARI
{
	public class ARIBaseAction
	{
		private readonly IActionConsumer _consumer;

		public ARIBaseAction(IActionConsumer consumer)
		{
			_consumer = consumer;
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected IRestCommand GetNewRequest(string requestString, HttpMethod method)
		{
			return _consumer.GetRestCommand(method, requestString);
		}

        protected  async Task<IRestCommandResult<T>> ExecuteAsync<T>(IRestCommand command, CancellationToken cancellationToken = default) where T : new()
        {
            return await _consumer.ProcessRestCommandAsync<T>(command, cancellationToken);
        }

        protected async Task<IRestCommandResult> ExecuteAsync(IRestCommand command, CancellationToken cancellationToken = default)
        {
            return await _consumer.ProcessRestCommandAsync(command, cancellationToken);
        }
	}
}