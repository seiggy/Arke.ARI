/*
   Arke ARI Framework
   Automatically generated file @ 8/2/2023 11:01:41 AM
*/
using System.Collections.Generic;
using System.Linq;
using Arke.ARI.Middleware;
using Arke.ARI.Models;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public class EventsActions : ARIBaseAction, IEventsActions
    {

        public EventsActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// WebSocket connection for events.. 
        /// </summary>
        public virtual async Task<Message> EventWebsocketAsync(string app, bool? subscribeAll = null)
        {
            string path = "events";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (subscribeAll != null)
                request.AddParameter("subscribeAll", subscribeAll, ParameterType.QueryString);

            var response = await ExecuteAsync<Message>(request);

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
        /// Generate a user event.. 
        /// </summary>
        public virtual async Task UserEventAsync(string eventName, string application, string source = null, Dictionary<string, string> variables = null)
        {
            string path = "events/user/{eventName}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (eventName != null)
                request.AddUrlSegment("eventName", eventName);
            if (application != null)
                request.AddParameter("application", application, ParameterType.QueryString);
            if (source != null)
                request.AddParameter("source", source, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Application does not exist.", (int)response.StatusCode);
                case 422:
                    throw new AriException("Event source not found.", (int)response.StatusCode);
                case 400:
                    throw new AriException("Invalid even tsource URI or userevent data.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

