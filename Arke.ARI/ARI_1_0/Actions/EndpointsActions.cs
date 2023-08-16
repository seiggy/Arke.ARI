/*
   Arke ARI Framework
   Automatically generated file @ 8/16/2023 10:25:28 AM
*/
using System.Collections.Generic;
using System.Linq;
using Arke.ARI.Middleware;
using Arke.ARI.Models;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public class EndpointsActions : ARIBaseAction, IEndpointsActions
    {

        public EndpointsActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// List all endpoints.. 
        /// </summary>
        public virtual async Task<List<Endpoint>> ListAsync()
        {
            string path = "endpoints";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<List<Endpoint>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Send a message to some technology URI or endpoint.. 
        /// </summary>
        public virtual async Task SendMessageAsync(string to, string from, string body = null, Dictionary<string, string> variables = null)
        {
            string path = "endpoints/sendMessage";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (to != null)
                request.AddParameter("to", to, ParameterType.QueryString);
            if (from != null)
                request.AddParameter("from", from, ParameterType.QueryString);
            if (body != null)
                request.AddParameter("body", body, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters for sending a message.", (int)response.StatusCode);
                case 404:
                    throw new AriException("Endpoint not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// List available endoints for a given endpoint technology.. 
        /// </summary>
        public virtual async Task<List<Endpoint>> ListByTechAsync(string tech)
        {
            string path = "endpoints/{tech}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (tech != null)
                request.AddUrlSegment("tech", tech);

            var response = await ExecuteAsync<List<Endpoint>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Endpoints not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Details for an endpoint.. 
        /// </summary>
        public virtual async Task<Endpoint> GetAsync(string tech, string resource)
        {
            string path = "endpoints/{tech}/{resource}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (tech != null)
                request.AddUrlSegment("tech", tech);
            if (resource != null)
                request.AddUrlSegment("resource", resource);

            var response = await ExecuteAsync<Endpoint>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters for sending a message.", (int)response.StatusCode);
                case 404:
                    throw new AriException("Endpoints not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Send a message to some endpoint in a technology.. 
        /// </summary>
        public virtual async Task SendMessageToEndpointAsync(string tech, string resource, string from, string body = null, Dictionary<string, string> variables = null)
        {
            string path = "endpoints/{tech}/{resource}/sendMessage";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (tech != null)
                request.AddUrlSegment("tech", tech);
            if (resource != null)
                request.AddUrlSegment("resource", resource);
            if (from != null)
                request.AddParameter("from", from, ParameterType.QueryString);
            if (body != null)
                request.AddParameter("body", body, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters for sending a message.", (int)response.StatusCode);
                case 404:
                    throw new AriException("Endpoint not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

