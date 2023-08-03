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

    public interface ISoundsActions
    {
        /// <summary>
        /// List all sounds.. 
        /// </summary>
        /// <param name="lang">Lookup sound for a specific language.</param>
        /// <param name="format">Lookup sound in a specific format.</param>
        Task<List<Sound>> ListAsync(string lang = null, string format = null);
        /// <summary>
        /// Get a sound's details.. 
        /// </summary>
        /// <param name="soundId">Sound's id</param>
        Task<Sound> GetAsync(string soundId);
    }
}
