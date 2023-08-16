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
    /// Notification that a channel has left a bridge.
    /// </summary>
    public class ChannelLeftBridgeEvent : Event
    {


        /// <summary>
        /// no description provided
        /// </summary>
        public Bridge Bridge { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public Channel Channel { get; set; }

    }
}
