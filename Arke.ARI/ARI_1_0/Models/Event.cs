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
    /// Base type for asynchronous events from Asterisk.
    /// </summary>
    public class Event : Message
    {


        /// <summary>
        /// Name of the application receiving the event.
        /// </summary>
        public string Application { get; set; }

        /// <summary>
        /// Time at which this event was created.
        /// </summary>
        public DateTime Timestamp { get; set; }

    }
}
