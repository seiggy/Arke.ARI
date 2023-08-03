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
    /// Notification that trying to move a channel to another Stasis application failed.
    /// </summary>
    public class ApplicationMoveFailedEvent : Event
    {


        /// <summary>
        /// no description provided
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Arguments to the application
        /// </summary>
        public List<string> Args { get; set; }

    }
}
