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

    public class AsteriskActions : ARIBaseAction, IAsteriskActions
    {

        public AsteriskActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// Retrieve a dynamic configuration object.. 
        /// </summary>
        public virtual async Task<List<ConfigTuple>> GetObjectAsync(string configClass, string objectType, string id)
        {
            string path = "asterisk/config/dynamic/{configClass}/{objectType}/{id}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (configClass != null)
                request.AddUrlSegment("configClass", configClass);
            if (objectType != null)
                request.AddUrlSegment("objectType", objectType);
            if (id != null)
                request.AddUrlSegment("id", id);

            var response = await ExecuteAsync<List<ConfigTuple>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("{configClass|objectType|id} not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Create or update a dynamic configuration object.. 
        /// </summary>
        public virtual async Task<List<ConfigTuple>> UpdateObjectAsync(string configClass, string objectType, string id, Dictionary<string, string> fields = null)
        {
            string path = "asterisk/config/dynamic/{configClass}/{objectType}/{id}";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (configClass != null)
                request.AddUrlSegment("configClass", configClass);
            if (objectType != null)
                request.AddUrlSegment("objectType", objectType);
            if (id != null)
                request.AddUrlSegment("id", id);
            if (fields != null)
            {
                request.AddParameter("application/json", new { fields = fields }, ParameterType.RequestBody);
            }

            var response = await ExecuteAsync<List<ConfigTuple>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Bad request body", (int)response.StatusCode);
                case 403:
                    throw new AriException("Could not create or update object", (int)response.StatusCode);
                case 404:
                    throw new AriException("{configClass|objectType} not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Delete a dynamic configuration object.. 
        /// </summary>
        public virtual async Task DeleteObjectAsync(string configClass, string objectType, string id)
        {
            string path = "asterisk/config/dynamic/{configClass}/{objectType}/{id}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (configClass != null)
                request.AddUrlSegment("configClass", configClass);
            if (objectType != null)
                request.AddUrlSegment("objectType", objectType);
            if (id != null)
                request.AddUrlSegment("id", id);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 403:
                    throw new AriException("Could not delete object", (int)response.StatusCode);
                case 404:
                    throw new AriException("{configClass|objectType|id} not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Gets Asterisk system information.. 
        /// </summary>
        public virtual async Task<AsteriskInfo> GetInfoAsync(string only = null)
        {
            string path = "asterisk/info";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (only != null)
                request.AddParameter("only", only, ParameterType.QueryString);

            var response = await ExecuteAsync<AsteriskInfo>(request);

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
        /// Response pong message.. 
        /// </summary>
        public virtual async Task<AsteriskPing> PingAsync()
        {
            string path = "asterisk/ping";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<AsteriskPing>(request);

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
        /// List Asterisk modules.. 
        /// </summary>
        public virtual async Task<List<Module>> ListModulesAsync()
        {
            string path = "asterisk/modules";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<List<Module>>(request);

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
        /// Get Asterisk module information.. 
        /// </summary>
        public virtual async Task<Module> GetModuleAsync(string moduleName)
        {
            string path = "asterisk/modules/{moduleName}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (moduleName != null)
                request.AddUrlSegment("moduleName", moduleName);

            var response = await ExecuteAsync<Module>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Module could not be found in running modules.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Module information could not be retrieved.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Load an Asterisk module.. 
        /// </summary>
        public virtual async Task LoadModuleAsync(string moduleName)
        {
            string path = "asterisk/modules/{moduleName}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (moduleName != null)
                request.AddUrlSegment("moduleName", moduleName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 409:
                    throw new AriException("Module could not be loaded.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Unload an Asterisk module.. 
        /// </summary>
        public virtual async Task UnloadModuleAsync(string moduleName)
        {
            string path = "asterisk/modules/{moduleName}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (moduleName != null)
                request.AddUrlSegment("moduleName", moduleName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Module not found in running modules.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Module could not be unloaded.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Reload an Asterisk module.. 
        /// </summary>
        public virtual async Task ReloadModuleAsync(string moduleName)
        {
            string path = "asterisk/modules/{moduleName}";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (moduleName != null)
                request.AddUrlSegment("moduleName", moduleName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Module not found in running modules.", (int)response.StatusCode);
                case 409:
                    throw new AriException("Module could not be reloaded.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Gets Asterisk log channel information.. 
        /// </summary>
        public virtual async Task<List<LogChannel>> ListLogChannelsAsync()
        {
            string path = "asterisk/logging";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<List<LogChannel>>(request);

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
        /// Adds a log channel.. 
        /// </summary>
        public virtual async Task AddLogAsync(string logChannelName, string configuration)
        {
            string path = "asterisk/logging/{logChannelName}";
            var request = GetNewRequest(path, HttpMethod.POST);
            if (logChannelName != null)
                request.AddUrlSegment("logChannelName", logChannelName);
            if (configuration != null)
                request.AddParameter("configuration", configuration, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Bad request body", (int)response.StatusCode);
                case 409:
                    throw new AriException("Log channel could not be created.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Deletes a log channel.. 
        /// </summary>
        public virtual async Task DeleteLogAsync(string logChannelName)
        {
            string path = "asterisk/logging/{logChannelName}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (logChannelName != null)
                request.AddUrlSegment("logChannelName", logChannelName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Log channel does not exist.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Rotates a log channel.. 
        /// </summary>
        public virtual async Task RotateLogAsync(string logChannelName)
        {
            string path = "asterisk/logging/{logChannelName}/rotate";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (logChannelName != null)
                request.AddUrlSegment("logChannelName", logChannelName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Log channel does not exist.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Get the value of a global variable.. 
        /// </summary>
        public virtual async Task<Variable> GetGlobalVarAsync(string variable)
        {
            string path = "asterisk/variable";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (variable != null)
                request.AddParameter("variable", variable, ParameterType.QueryString);

            var response = await ExecuteAsync<Variable>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 400:
                    throw new AriException("Missing variable parameter.", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Set the value of a global variable.. 
        /// </summary>
        public virtual async Task SetGlobalVarAsync(string variable, string value = null)
        {
            string path = "asterisk/variable";
            var request = GetNewRequest(path, HttpMethod.POST);
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
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

