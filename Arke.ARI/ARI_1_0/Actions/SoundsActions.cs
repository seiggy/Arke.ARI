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

    public class SoundsActions : ARIBaseAction, ISoundsActions
    {

        public SoundsActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// List all sounds.. 
        /// </summary>
        public virtual async Task<List<Sound>> ListAsync(string lang = null, string format = null)
        {
            string path = "sounds";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (lang != null)
                request.AddParameter("lang", lang, ParameterType.QueryString);
            if (format != null)
                request.AddParameter("format", format, ParameterType.QueryString);

            var response = await ExecuteAsync<List<Sound>>(request);

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
        /// Get a sound's details.. 
        /// </summary>
        public virtual async Task<Sound> GetAsync(string soundId)
        {
            string path = "sounds/{soundId}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (soundId != null)
                request.AddUrlSegment("soundId", soundId);

            var response = await ExecuteAsync<Sound>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

