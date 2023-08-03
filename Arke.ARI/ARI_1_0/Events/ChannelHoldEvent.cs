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
    /// A channel initiated a media hold.
    /// </summary>
    public class ChannelHoldEvent : Event
    {


        /// <summary>
        /// The channel that initiated the hold event.
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// The music on hold class that the initiator requested.
        /// </summary>
        public string Musicclass { get; set; }

    }
}
