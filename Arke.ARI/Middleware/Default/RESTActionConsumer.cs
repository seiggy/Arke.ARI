using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arke.ARI.Middleware.Default
{
    public class RestActionConsumer : IActionConsumer
    {
        private readonly StasisEndpoint _connectionInfo;
        private readonly IServiceProvider _serviceProvider;

        public RestActionConsumer(StasisEndpoint connectionInfo, IServiceProvider serviceProvider)
        {
            _connectionInfo = connectionInfo;
            _serviceProvider = serviceProvider;
        }

        public IRestCommand GetRestCommand(HttpMethod method, string path)
        {
            return new Command(_connectionInfo, path, _serviceProvider.GetService<IHttpClientFactory>(), _serviceProvider.GetService<ILogger<Command>>())
            {
                UniqueId = Guid.NewGuid().ToString(),
                Method = method
            };
        }

        public async Task<IRestCommandResult<T>> ProcessRestCommandAsync<T>(IRestCommand command, CancellationToken cancellationToken) where T : new()
        {
            return await command.ExecuteAsync<T>(cancellationToken);
        }

        public async Task<IRestCommandResult> ProcessRestCommandAsync(IRestCommand command, CancellationToken cancellationToken)
        {
            return await command.ExecuteAsync(cancellationToken);
        }
    }
}