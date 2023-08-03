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
    /// Channel changed Connected Line.
    /// </summary>
    public class ChannelConnectedLineEvent : Event
    {


        /// <summary>
        /// The channel whose connected line has changed.
        /// </summary>
        public Channel Channel { get; set; }

    }
}
