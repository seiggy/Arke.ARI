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

    public class DeviceStatesActions : ARIBaseAction, IDeviceStatesActions
    {

        public DeviceStatesActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// List all ARI controlled device states.. 
        /// </summary>
        public virtual async Task<List<DeviceState>> ListAsync()
        {
            string path = "deviceStates";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteAsync<List<DeviceState>>(request);

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
        /// Retrieve the current state of a device.. 
        /// </summary>
        public virtual async Task<DeviceState> GetAsync(string deviceName)
        {
            string path = "deviceStates/{deviceName}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (deviceName != null)
                request.AddUrlSegment("deviceName", deviceName);

            var response = await ExecuteAsync<DeviceState>(request);

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
        /// Change the state of a device controlled by ARI. (Note - implicitly creates the device state).. 
        /// </summary>
        public virtual async Task UpdateAsync(string deviceName, string deviceState)
        {
            string path = "deviceStates/{deviceName}";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (deviceName != null)
                request.AddUrlSegment("deviceName", deviceName);
            if (deviceState != null)
                request.AddParameter("deviceState", deviceState, ParameterType.QueryString);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Device name is missing", (int)response.StatusCode);
                case 409:
                    throw new AriException("Uncontrolled device specified", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Destroy a device-state controlled by ARI.. 
        /// </summary>
        public virtual async Task DeleteAsync(string deviceName)
        {
            string path = "deviceStates/{deviceName}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (deviceName != null)
                request.AddUrlSegment("deviceName", deviceName);
            var response = await ExecuteAsync(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Device name is missing", (int)response.StatusCode);
                case 409:
                    throw new AriException("Uncontrolled device specified", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

