/*
   Arke ARI Framework
   Automatically generated file @ 8/2/2023 11:01:41 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Models;
using Arke.ARI;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public interface IAsteriskActions
    {
        /// <summary>
        /// Retrieve a dynamic configuration object.. 
        /// </summary>
        /// <param name="configClass">The configuration class containing dynamic configuration objects.</param>
        /// <param name="objectType">The type of configuration object to retrieve.</param>
        /// <param name="id">The unique identifier of the object to retrieve.</param>
        Task<List<ConfigTuple>> GetObjectAsync(string configClass, string objectType, string id);
        /// <summary>
        /// Create or update a dynamic configuration object.. 
        /// </summary>
        /// <param name="configClass">The configuration class containing dynamic configuration objects.</param>
        /// <param name="objectType">The type of configuration object to create or update.</param>
        /// <param name="id">The unique identifier of the object to create or update.</param>
        /// <param name="fields">The body object should have a value that is a list of ConfigTuples, which provide the fields to update. Ex. [ { "attribute": "directmedia", "value": "false" } ]</param>
        Task<List<ConfigTuple>> UpdateObjectAsync(string configClass, string objectType, string id, Dictionary<string, string> fields = null);
        /// <summary>
        /// Delete a dynamic configuration object.. 
        /// </summary>
        /// <param name="configClass">The configuration class containing dynamic configuration objects.</param>
        /// <param name="objectType">The type of configuration object to delete.</param>
        /// <param name="id">The unique identifier of the object to delete.</param>
        Task DeleteObjectAsync(string configClass, string objectType, string id);
        /// <summary>
        /// Gets Asterisk system information.. 
        /// </summary>
        /// <param name="only">Filter information returned</param>
        Task<AsteriskInfo> GetInfoAsync(string only = null);
        /// <summary>
        /// Response pong message.. 
        /// </summary>
        Task<AsteriskPing> PingAsync();
        /// <summary>
        /// List Asterisk modules.. 
        /// </summary>
        Task<List<Module>> ListModulesAsync();
        /// <summary>
        /// Get Asterisk module information.. 
        /// </summary>
        /// <param name="moduleName">Module's name</param>
        Task<Module> GetModuleAsync(string moduleName);
        /// <summary>
        /// Load an Asterisk module.. 
        /// </summary>
        /// <param name="moduleName">Module's name</param>
        Task LoadModuleAsync(string moduleName);
        /// <summary>
        /// Unload an Asterisk module.. 
        /// </summary>
        /// <param name="moduleName">Module's name</param>
        Task UnloadModuleAsync(string moduleName);
        /// <summary>
        /// Reload an Asterisk module.. 
        /// </summary>
        /// <param name="moduleName">Module's name</param>
        Task ReloadModuleAsync(string moduleName);
        /// <summary>
        /// Gets Asterisk log channel information.. 
        /// </summary>
        Task<List<LogChannel>> ListLogChannelsAsync();
        /// <summary>
        /// Adds a log channel.. 
        /// </summary>
        /// <param name="logChannelName">The log channel to add</param>
        /// <param name="configuration">levels of the log channel</param>
        Task AddLogAsync(string logChannelName, string configuration);
        /// <summary>
        /// Deletes a log channel.. 
        /// </summary>
        /// <param name="logChannelName">Log channels name</param>
        Task DeleteLogAsync(string logChannelName);
        /// <summary>
        /// Rotates a log channel.. 
        /// </summary>
        /// <param name="logChannelName">Log channel's name</param>
        Task RotateLogAsync(string logChannelName);
        /// <summary>
        /// Get the value of a global variable.. 
        /// </summary>
        /// <param name="variable">The variable to get</param>
        Task<Variable> GetGlobalVarAsync(string variable);
        /// <summary>
        /// Set the value of a global variable.. 
        /// </summary>
        /// <param name="variable">The variable to set</param>
        /// <param name="value">The value to set the variable to</param>
        Task SetGlobalVarAsync(string variable, string value = null);
    }
}
