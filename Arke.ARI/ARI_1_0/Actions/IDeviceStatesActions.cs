/*
   Arke ARI Framework
   Automatically generated file @ 8/16/2023 10:25:28 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Models;
using Arke.ARI;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public interface IDeviceStatesActions
    {
        /// <summary>
        /// List all ARI controlled device states.. 
        /// </summary>
        Task<List<DeviceState>> ListAsync();
        /// <summary>
        /// Retrieve the current state of a device.. 
        /// </summary>
        /// <param name="deviceName">Name of the device</param>
        Task<DeviceState> GetAsync(string deviceName);
        /// <summary>
        /// Change the state of a device controlled by ARI. (Note - implicitly creates the device state).. 
        /// </summary>
        /// <param name="deviceName">Name of the device</param>
        /// <param name="deviceState">Device state value</param>
        Task UpdateAsync(string deviceName, string deviceState);
        /// <summary>
        /// Destroy a device-state controlled by ARI.. 
        /// </summary>
        /// <param name="deviceName">Name of the device</param>
        Task DeleteAsync(string deviceName);
    }
}
