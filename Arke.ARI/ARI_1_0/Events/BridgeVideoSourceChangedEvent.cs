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
    /// Notification that the source of video in a bridge has changed.
    /// </summary>
    public class BridgeVideoSourceChangedEvent : Event
    {


        /// <summary>
        /// no description provided
        /// </summary>
        public Bridge Bridge { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public string Old_video_source_id { get; set; }

    }
}
