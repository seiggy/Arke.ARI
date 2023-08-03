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
    /// Notification that one bridge has merged into another.
    /// </summary>
    public class BridgeMergedEvent : Event
    {


        /// <summary>
        /// no description provided
        /// </summary>
        public Bridge Bridge { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public Bridge Bridge_from { get; set; }

    }
}
