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

    public class ChannelsActions : ARIBaseAction, IChannelsActions
    {

        public ChannelsActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// List all active channels in Asterisk.. 
        /// </summary>
        public virtual async Task<List<Channel>> ListAsync()
        {
            string path = "channels";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<List<Channel>>(request);

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
        /// Create a new channel (originate).. The new channel is created immediately and a snapshot of it returned. If a Stasis application is provided it will be automatically subscribed to the originated channel for further events and updates.
        /// </summary>
        public virtual async Task<Channel> OriginateAsync(string endpoint, string extension = null, string context = null, long? priority = null, string label = null, string app = null, string appArgs = null, string callerId = null, int? timeout = null, Dictionary<string, string> variables = null, string channelId = null, string otherChannelId = null, string originator = null, string formats = null)
        {
            string path = "channels";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (endpoint != null)
                request.AddParameter("endpoint", endpoint, ParameterType.QueryString);
            if (extension != null)
                request.AddParameter("extension", extension, ParameterType.QueryString);
            if (context != null)
                request.AddParameter("context", context, ParameterType.QueryString);
            if (priority != null)
                request.AddParameter("priority", priority, ParameterType.QueryString);
            if (label != null)
                request.AddParameter("label", label, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);
            if (callerId != null)
                request.AddParameter("callerId", callerId, ParameterType.QueryString);
            if (timeout != null)
                request.AddParameter("timeout", timeout, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            if (channelId != null)
                request.AddParameter("channelId", channelId, ParameterType.QueryString);
            if (otherChannelId != null)
                request.AddParameter("otherChannelId", otherChannelId, ParameterType.QueryString);
            if (originator != null)
                request.AddParameter("originator", originator, ParameterType.QueryString);
            if (formats != null)
                request.AddParameter("formats", formats, ParameterType.QueryString);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters for originating a channel.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel with given unique ID already exists.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Create channel.. 
        /// </summary>
        public virtual async Task<Channel> CreateAsync(string endpoint, string app, string appArgs = null, string channelId = null, string otherChannelId = null, string originator = null, string formats = null, Dictionary<string, string> variables = null)
        {
            string path = "channels/create";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (endpoint != null)
                request.AddParameter("endpoint", endpoint, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);
            if (channelId != null)
                request.AddParameter("channelId", channelId, ParameterType.QueryString);
            if (otherChannelId != null)
                request.AddParameter("otherChannelId", otherChannelId, ParameterType.QueryString);
            if (originator != null)
                request.AddParameter("originator", originator, ParameterType.QueryString);
            if (formats != null)
                request.AddParameter("formats", formats, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 409:
                    throw new AriException("Channel with given unique ID already exists.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Channel details.. 
        /// </summary>
        public virtual async Task<Channel> GetAsync(string channelId)
        {
            string path = "channels/{channelId}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Create a new channel (originate with id).. The new channel is created immediately and a snapshot of it returned. If a Stasis application is provided it will be automatically subscribed to the originated channel for further events and updates.
        /// </summary>
        public virtual async Task<Channel> OriginateWithIdAsync(string channelId, string endpoint, string extension = null, string context = null, long? priority = null, string label = null, string app = null, string appArgs = null, string callerId = null, int? timeout = null, Dictionary<string, string> variables = null, string otherChannelId = null, string originator = null, string formats = null)
        {
            string path = "channels/{channelId}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (endpoint != null)
                request.AddParameter("endpoint", endpoint, ParameterType.QueryString);
            if (extension != null)
                request.AddParameter("extension", extension, ParameterType.QueryString);
            if (context != null)
                request.AddParameter("context", context, ParameterType.QueryString);
            if (priority != null)
                request.AddParameter("priority", priority, ParameterType.QueryString);
            if (label != null)
                request.AddParameter("label", label, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);
            if (callerId != null)
                request.AddParameter("callerId", callerId, ParameterType.QueryString);
            if (timeout != null)
                request.AddParameter("timeout", timeout, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            if (otherChannelId != null)
                request.AddParameter("otherChannelId", otherChannelId, ParameterType.QueryString);
            if (originator != null)
                request.AddParameter("originator", originator, ParameterType.QueryString);
            if (formats != null)
                request.AddParameter("formats", formats, ParameterType.QueryString);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters for originating a channel.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel with given unique ID already exists.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Delete (i.e. hangup) a channel.. 
        /// </summary>
        public virtual async Task HangupAsync(string channelId, string reason_code = null, string reason = null)
        {
            string path = "channels/{channelId}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (reason_code != null)
                request.AddParameter("reason_code", reason_code, ParameterType.QueryString);
            if (reason != null)
                request.AddParameter("reason", reason, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid reason for hangup provided", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Exit application; continue execution in the dialplan.. 
        /// </summary>
        public virtual async Task ContinueInDialplanAsync(string channelId, string context = null, string extension = null, int? priority = null, string label = null)
        {
            string path = "channels/{channelId}/continue";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (context != null)
                request.AddParameter("context", context, ParameterType.QueryString);
            if (extension != null)
                request.AddParameter("extension", extension, ParameterType.QueryString);
            if (priority != null)
                request.AddParameter("priority", priority, ParameterType.QueryString);
            if (label != null)
                request.AddParameter("label", label, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Move the channel from one Stasis application to another.. 
        /// </summary>
        public virtual async Task MoveAsync(string channelId, string app, string appArgs = null)
        {
            string path = "channels/{channelId}/move";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Redirect the channel to a different location.. 
        /// </summary>
        public virtual async Task RedirectAsync(string channelId, string endpoint)
        {
            string path = "channels/{channelId}/redirect";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (endpoint != null)
                request.AddParameter("endpoint", endpoint, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Endpoint parameter not provided", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel or endpoint not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 422:
                    throw new AriException("Endpoint is not the same type as the channel", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Answer a channel.. 
        /// </summary>
        public virtual async Task AnswerAsync(string channelId)
        {
            string path = "channels/{channelId}/answer";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Indicate ringing to a channel.. 
        /// </summary>
        public virtual async Task RingAsync(string channelId)
        {
            string path = "channels/{channelId}/ring";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Stop ringing indication on a channel if locally generated.. 
        /// </summary>
        public virtual async Task RingStopAsync(string channelId)
        {
            string path = "channels/{channelId}/ring";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Send provided DTMF to a given channel.. 
        /// </summary>
        public virtual async Task SendDTMFAsync(string channelId, string dtmf = null, int? before = null, int? between = null, int? duration = null, int? after = null)
        {
            string path = "channels/{channelId}/dtmf";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (dtmf != null)
                request.AddParameter("dtmf", dtmf, ParameterType.QueryString);
            if (before != null)
                request.AddParameter("before", before, ParameterType.QueryString);
            if (between != null)
                request.AddParameter("between", between, ParameterType.QueryString);
            if (duration != null)
                request.AddParameter("duration", duration, ParameterType.QueryString);
            if (after != null)
                request.AddParameter("after", after, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("DTMF is required", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Mute a channel.. 
        /// </summary>
        public virtual async Task MuteAsync(string channelId, string direction = null)
        {
            string path = "channels/{channelId}/mute";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (direction != null)
                request.AddParameter("direction", direction, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Unmute a channel.. 
        /// </summary>
        public virtual async Task UnmuteAsync(string channelId, string direction = null)
        {
            string path = "channels/{channelId}/mute";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (direction != null)
                request.AddParameter("direction", direction, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Hold a channel.. 
        /// </summary>
        public virtual async Task HoldAsync(string channelId)
        {
            string path = "channels/{channelId}/hold";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Remove a channel from hold.. 
        /// </summary>
        public virtual async Task UnholdAsync(string channelId)
        {
            string path = "channels/{channelId}/hold";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Play music on hold to a channel.. Using media operations such as /play on a channel playing MOH in this manner will suspend MOH without resuming automatically. If continuing music on hold is desired, the stasis application must reinitiate music on hold.
        /// </summary>
        public virtual async Task StartMohAsync(string channelId, string mohClass = null)
        {
            string path = "channels/{channelId}/moh";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (mohClass != null)
                request.AddParameter("mohClass", mohClass, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Stop playing music on hold to a channel.. 
        /// </summary>
        public virtual async Task StopMohAsync(string channelId)
        {
            string path = "channels/{channelId}/moh";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Play silence to a channel.. Using media operations such as /play on a channel playing silence in this manner will suspend silence without resuming automatically.
        /// </summary>
        public virtual async Task StartSilenceAsync(string channelId)
        {
            string path = "channels/{channelId}/silence";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Stop playing silence to a channel.. 
        /// </summary>
        public virtual async Task StopSilenceAsync(string channelId)
        {
            string path = "channels/{channelId}/silence";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start playback of media.. The media URI may be any of a number of URI's. Currently sound:, recording:, number:, digits:, characters:, and tone: URI's are supported. This operation creates a playback resource that can be used to control the playback of media (pause, rewind, fast forward, etc.)
        /// </summary>
        public virtual async Task<Playback> PlayAsync(string channelId, string media, string lang = null, int? offsetms = null, int? skipms = null, string playbackId = null)
        {
            string path = "channels/{channelId}/play";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (media != null)
                request.AddParameter("media", media, ParameterType.QueryString);
            if (lang != null)
                request.AddParameter("lang", lang, ParameterType.QueryString);
            if (offsetms != null)
                request.AddParameter("offsetms", offsetms, ParameterType.QueryString);
            if (skipms != null)
                request.AddParameter("skipms", skipms, ParameterType.QueryString);
            if (playbackId != null)
                request.AddParameter("playbackId", playbackId, ParameterType.QueryString);

            var response = await ExecuteAsync<Playback>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start playback of media and specify the playbackId.. The media URI may be any of a number of URI's. Currently sound:, recording:, number:, digits:, characters:, and tone: URI's are supported. This operation creates a playback resource that can be used to control the playback of media (pause, rewind, fast forward, etc.)
        /// </summary>
        public virtual async Task<Playback> PlayWithIdAsync(string channelId, string playbackId, string media, string lang = null, int? offsetms = null, int? skipms = null)
        {
            string path = "channels/{channelId}/play/{playbackId}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (playbackId != null)
                request.AddUrlSegment("playbackId", playbackId);
            if (media != null)
                request.AddParameter("media", media, ParameterType.QueryString);
            if (lang != null)
                request.AddParameter("lang", lang, ParameterType.QueryString);
            if (offsetms != null)
                request.AddParameter("offsetms", offsetms, ParameterType.QueryString);
            if (skipms != null)
                request.AddParameter("skipms", skipms, ParameterType.QueryString);

            var response = await ExecuteAsync<Playback>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                case 412:
                    throw new AriException("Channel in invalid state", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start a recording.. Record audio from a channel. Note that this will not capture audio sent to the channel. The bridge itself has a record feature if that's what you want.
        /// </summary>
        public virtual async Task<LiveRecording> RecordAsync(string channelId, string name, string format, int? maxDurationSeconds = null, int? maxSilenceSeconds = null, string ifExists = null, bool? beep = null, string terminateOn = null)
        {
            string path = "channels/{channelId}/record";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (name != null)
                request.AddParameter("name", name, ParameterType.QueryString);
            if (format != null)
                request.AddParameter("format", format, ParameterType.QueryString);
            if (maxDurationSeconds != null)
                request.AddParameter("maxDurationSeconds", maxDurationSeconds, ParameterType.QueryString);
            if (maxSilenceSeconds != null)
                request.AddParameter("maxSilenceSeconds", maxSilenceSeconds, ParameterType.QueryString);
            if (ifExists != null)
                request.AddParameter("ifExists", ifExists, ParameterType.QueryString);
            if (beep != null)
                request.AddParameter("beep", beep, ParameterType.QueryString);
            if (terminateOn != null)
                request.AddParameter("terminateOn", terminateOn, ParameterType.QueryString);

            var response = await ExecuteAsync<LiveRecording>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel is not in a Stasis application; the channel is currently bridged with other hcannels; A recording with the same name already exists on the system and can not be overwritten because it is in progress or ifExists=fail", (int)response.StatusCode);
                case 422:
                    throw new AriException("The format specified is unknown on this system", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Get the value of a channel variable or function.. 
        /// </summary>
        public virtual async Task<Variable> GetChannelVarAsync(string channelId, string variable)
        {
            string path = "channels/{channelId}/variable";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (variable != null)
                request.AddParameter("variable", variable, ParameterType.QueryString);

            var response = await ExecuteAsync<Variable>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Missing variable parameter.", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel or variable not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Set the value of a channel variable or function.. 
        /// </summary>
        public virtual async Task SetChannelVarAsync(string channelId, string variable, string value = null)
        {
            string path = "channels/{channelId}/variable";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (variable != null)
                request.AddParameter("variable", variable, ParameterType.QueryString);
            if (value != null)
                request.AddParameter("value", value, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Missing variable parameter.", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel not in a Stasis application", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start snooping.. Snoop (spy/whisper) on a specific channel.
        /// </summary>
        public virtual async Task<Channel> SnoopChannelAsync(string channelId, string app, string spy = null, string whisper = null, string appArgs = null, string snoopId = null)
        {
            string path = "channels/{channelId}/snoop";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (spy != null)
                request.AddParameter("spy", spy, ParameterType.QueryString);
            if (whisper != null)
                request.AddParameter("whisper", whisper, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);
            if (snoopId != null)
                request.AddParameter("snoopId", snoopId, ParameterType.QueryString);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start snooping.. Snoop (spy/whisper) on a specific channel.
        /// </summary>
        public virtual async Task<Channel> SnoopChannelWithIdAsync(string channelId, string snoopId, string app, string spy = null, string whisper = null, string appArgs = null)
        {
            string path = "channels/{channelId}/snoop/{snoopId}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (snoopId != null)
                request.AddUrlSegment("snoopId", snoopId);
            if (spy != null)
                request.AddParameter("spy", spy, ParameterType.QueryString);
            if (whisper != null)
                request.AddParameter("whisper", whisper, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (appArgs != null)
                request.AddParameter("appArgs", appArgs, ParameterType.QueryString);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters", (int)response.StatusCode);
                case 404:
                    throw new AriException("Channel not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Dial a created channel.. 
        /// </summary>
        public virtual async Task DialAsync(string channelId, string caller = null, int? timeout = null)
        {
            string path = "channels/{channelId}/dial";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);
            if (caller != null)
                request.AddParameter("caller", caller, ParameterType.QueryString);
            if (timeout != null)
                request.AddParameter("timeout", timeout, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel cannot be found.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel cannot be dialed.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// RTP stats on a channel.. 
        /// </summary>
        public virtual async Task<RTPstat> RtpstatisticsAsync(string channelId)
        {
            string path = "channels/{channelId}/rtp_statistics";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (channelId != null)
                request.AddUrlSegment("channelId", channelId);

            var response = await ExecuteAsync<RTPstat>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Channel cannot be found.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Start an External Media session.. Create a channel to an External Media source/sink.
        /// </summary>
        public virtual async Task<Channel> ExternalMediaAsync(string app, string external_host, string format, string channelId = null, Dictionary<string, string> variables = null, string encapsulation = null, string transport = null, string connection_type = null, string direction = null, string data = null)
        {
            string path = "channels/externalMedia";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (channelId != null)
                request.AddParameter("channelId", channelId, ParameterType.QueryString);
            if (app != null)
                request.AddParameter("app", app, ParameterType.QueryString);
            if (variables != null)
            {
                request.AddParameter("application/json", new { variables = variables }, ParameterType.RequestBody);
            }
            if (external_host != null)
                request.AddParameter("external_host", external_host, ParameterType.QueryString);
            if (encapsulation != null)
                request.AddParameter("encapsulation", encapsulation, ParameterType.QueryString);
            if (transport != null)
                request.AddParameter("transport", transport, ParameterType.QueryString);
            if (connection_type != null)
                request.AddParameter("connection_type", connection_type, ParameterType.QueryString);
            if (format != null)
                request.AddParameter("format", format, ParameterType.QueryString);
            if (direction != null)
                request.AddParameter("direction", direction, ParameterType.QueryString);
            if (data != null)
                request.AddParameter("data", data, ParameterType.QueryString);

            var response = await ExecuteAsync<Channel>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Invalid parameters", (int)response.StatusCode);
                case 409:
                    throw new AriException("Channel is not in a Stasis application; Channel is already bridged", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

