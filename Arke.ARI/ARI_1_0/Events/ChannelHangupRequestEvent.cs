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
    /// A hangup was requested on the channel.
    /// </summary>
    public class ChannelHangupRequestEvent : Event
    {


        /// <summary>
        /// Integer representation of the cause of the hangup.
        /// </summary>
        public int Cause { get; set; }

        /// <summary>
        /// Whether the hangup request was a soft hangup request.
        /// </summary>
        public bool Soft { get; set; }

        /// <summary>
        /// The channel on which the hangup was requested.
        /// </summary>
        public Channel Channel { get; set; }

    }
}
