/*
   Arke ARI Framework
   Automatically generated file @ 8/2/2023 11:01:40 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Actions;

namespace Arke.ARI.Models
{
    /// <summary>
    /// Event showing the completion of a media playback operation.
    /// </summary>
    public class PlaybackFinishedEvent : Event
    {


        /// <summary>
        /// Playback control object
        /// </summary>
        public Playback Playback { get; set; }

    }
}
