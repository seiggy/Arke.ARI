using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware
{

    public enum ParameterType
    {
        RequestBody,
        QueryString
    }

    public interface IRestCommand
    {
        string UniqueId { get; set; }
        string Url { get; }
        HttpMethod Method { get; set; }
        string Body { get; }

        void AddUrlSegment(string segName, string value);
        void AddParameter(string name, object value, ParameterType type);
        Task<IRestCommandResult<T>> ExecuteAsync<T>(CancellationToken cancellationToken) where T : new();
        Task<IRestCommandResult> ExecuteAsync(CancellationToken cancellationToken);
    }
}