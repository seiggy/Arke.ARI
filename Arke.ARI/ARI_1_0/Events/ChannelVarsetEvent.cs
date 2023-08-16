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
    /// Channel variable changed.
    /// </summary>
    public class ChannelVarsetEvent : Event
    {


        /// <summary>
        /// The variable that changed.
        /// </summary>
        public string Variable { get; set; }

        /// <summary>
        /// The new value of the variable.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The channel on which the variable was set.  If missing, the variable is a global variable.
        /// </summary>
        public Channel Channel { get; set; }

    }
}
