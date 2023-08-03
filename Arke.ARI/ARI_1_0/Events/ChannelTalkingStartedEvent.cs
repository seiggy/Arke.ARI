/*
   Arke ARI Framework
   Automatically generated file @ 8/2/2023 11:01:41 AM
*/
using System;
using System.Collections.Generic;
using Arke.ARI.Actions;

namespace Arke.ARI.Models
{
    /// <summary>
    /// Talking was detected on the channel.
    /// </summary>
    public class ChannelTalkingStartedEvent : Event
    {


        /// <summary>
        /// The channel on which talking started.
        /// </summary>
        public Channel Channel { get; set; }

    }
}
