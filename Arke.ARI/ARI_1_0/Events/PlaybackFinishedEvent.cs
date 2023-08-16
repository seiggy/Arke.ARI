/*
   Arke ARI Framework
   Automatically generated file @ 8/16/2023 10:25:28 AM
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
