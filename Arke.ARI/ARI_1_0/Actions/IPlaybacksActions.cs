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

    public interface IPlaybacksActions
    {
        /// <summary>
        /// Get a playback's details.. 
        /// </summary>
        /// <param name="playbackId">Playback's id</param>
        Task<Playback> GetAsync(string playbackId);
        /// <summary>
        /// Stop a playback.. 
        /// </summary>
        /// <param name="playbackId">Playback's id</param>
        Task StopAsync(string playbackId);
        /// <summary>
        /// Control a playback.. 
        /// </summary>
        /// <param name="playbackId">Playback's id</param>
        /// <param name="operation">Operation to perform on the playback.</param>
        Task ControlAsync(string playbackId, string operation);
    }
}
