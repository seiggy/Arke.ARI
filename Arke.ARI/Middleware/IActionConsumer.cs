using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware
{

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD,
        OPTIONS,
        PATCH,
        MERGE
    }

    public interface IActionConsumer
    {
        IRestCommand GetRestCommand(HttpMethod method, string path);
        
        Task<IRestCommandResult<T>> ProcessRestCommandAsync<T>(IRestCommand command, CancellationToken cancellationToken) where T : new();
        Task<IRestCommandResult> ProcessRestCommandAsync(IRestCommand command, CancellationToken cancellationToken);
    }
}