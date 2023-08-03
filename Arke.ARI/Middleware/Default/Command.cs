using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Arke.ARI.Middleware.Default
{
    public class Command : IRestCommand
    {
        private readonly IHttpClientFactory _httpClientFactory = null!;
        private readonly ILogger<Command> _logger = null!;
        private readonly StasisEndpoint _stasisEndpoint = null!;
        private string _path;
        private Dictionary<string, string> _queryParameters;
        private StringContent _requestBody = null;
        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };


        public Command(StasisEndpoint info, string path, IHttpClientFactory httpClientFactory, ILogger<Command> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _stasisEndpoint = info;
            _path = path;
            _queryParameters = new Dictionary<string, string>();
            AddParameter("api_key", $"{info.Username}:{info.Password}", ParameterType.QueryString);
            _serializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());
        }


        public string UniqueId { get; set; }
        public string Url { 
            get
            {
                return $"{_stasisEndpoint.AriEndPoint}/{_path}";
            }
        }

        public HttpMethod Method { get; set; }


        public string Body { get; private set; }

        public void AddUrlSegment(string segName, string value)
        {
            _path = _path.Replace($"{{{segName}}}", UrlEncoder.Default.Encode(value));
        }

        public void AddParameter(string name, object value, Middleware.ParameterType type)
        {
            switch (type)
            {
                case ParameterType.RequestBody:
                    _requestBody = new(JsonSerializer.Serialize(value, _serializerOptions), Encoding.UTF8, "application/json");
                    break;
                case ParameterType.QueryString:
                    _queryParameters.Add(name, value.ToString());
                    break;
            }
        }

        private string GetUrlEncodedQueryString()
        {
            if (_queryParameters.Count == 0) { return string.Empty; }
            return string.Join("&", _queryParameters.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
        }

        public async Task<IRestCommandResult<T>> ExecuteAsync<T>(CancellationToken cancellationToken) where T : new()
        {
            switch (Method)
            {
                case HttpMethod.GET:
                    return await ExecuteGetAsync<T>(cancellationToken);
                case HttpMethod.POST:
                    return await ExecutePostAsync<T>(cancellationToken);
                case HttpMethod.PUT:
                    return await ExecutePutAsync<T>(cancellationToken);
                case HttpMethod.DELETE:
                    return await ExecuteDeleteAsync<T>(cancellationToken);
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<IRestCommandResult> ExecuteAsync(CancellationToken cancellationToken)
        {
            switch (Method)
            {
                case HttpMethod.GET:
                    return await ExecuteGetAsync(cancellationToken);
                case HttpMethod.POST:
                    return await ExecutePostAsync(cancellationToken);
                case HttpMethod.PUT:
                    return await ExecutePutAsync(cancellationToken);
                case HttpMethod.DELETE:
                    return await ExecuteDeleteAsync(cancellationToken);
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task<IRestCommandResult<T>> ExecuteGetAsync<T>(CancellationToken cancellationToken) where T : new()
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using var result = await client.GetAsync($"{Url}?{GetUrlEncodedQueryString()}", cancellationToken: cancellationToken);
                var data = await result.Content.ReadAsStringAsync();
                return new CommandResult<T>
                {
                    StatusCode = result.StatusCode,
                    Data = JsonSerializer.Deserialize<T>(data, _serializerOptions),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult> ExecuteGetAsync(CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using var result = await client.GetAsync($"{Url}?{GetUrlEncodedQueryString()}", cancellationToken: cancellationToken);
                return new CommandResult
                {
                    StatusCode = result.StatusCode,
                    RawData = await result.Content.ReadAsByteArrayAsync(),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult<T>> ExecutePostAsync<T>(CancellationToken cancellationToken) where T : new()
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.PostAsync($"{Url}?{GetUrlEncodedQueryString()}", _requestBody, cancellationToken: cancellationToken);
                var data = await result.Content.ReadAsStringAsync();
                return new CommandResult<T>
                {
                    StatusCode = result.StatusCode,
                    Data = JsonSerializer.Deserialize<T>(data, _serializerOptions),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult> ExecutePostAsync(CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.PostAsync($"{Url}?{GetUrlEncodedQueryString()}", _requestBody, cancellationToken: cancellationToken);
                return new CommandResult
                {
                    StatusCode = result.StatusCode,
                    RawData = await result.Content.ReadAsByteArrayAsync(),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult<T>> ExecutePutAsync<T>(CancellationToken cancellationToken) where T : new()
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.PutAsync($"{Url}?{GetUrlEncodedQueryString()}", _requestBody, cancellationToken: cancellationToken);
                return new CommandResult<T>
                {
                    StatusCode = result.StatusCode,
                    Data = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), _serializerOptions),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult> ExecutePutAsync(CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.PutAsync($"{Url}?{GetUrlEncodedQueryString()}", _requestBody, cancellationToken: cancellationToken);
                return new CommandResult
                {
                    StatusCode = result.StatusCode,
                    RawData = await result.Content.ReadAsByteArrayAsync(),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult<T>> ExecuteDeleteAsync<T>(CancellationToken cancellationToken) where T : new()
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.DeleteAsync($"{Url}?{GetUrlEncodedQueryString()}", cancellationToken: cancellationToken);
                return new CommandResult<T>
                {
                    StatusCode = result.StatusCode,
                    Data = JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync(), _serializerOptions),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }

        private async Task<IRestCommandResult> ExecuteDeleteAsync(CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();

            try
            {
                using HttpResponseMessage result = await client.DeleteAsync($"{Url}?{GetUrlEncodedQueryString()}", cancellationToken: cancellationToken);
                return new CommandResult
                {
                    StatusCode = result.StatusCode,
                    RawData = await result.Content.ReadAsByteArrayAsync(),
                    UniqueId = this.UniqueId
                };
            }
            catch (Exception e)
            {
                _logger.LogError("Error while executing the ARI command. Error: {0}, Command: {1}", e, this);
                throw;
            }
        }
    }
}
