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
    /// Info about Asterisk status
    /// </summary>
    public class StatusInfo
    {


        /// <summary>
        /// Time when Asterisk was started.
        /// </summary>
        public DateTime Startup_time { get; set; }

        /// <summary>
        /// Time when Asterisk was last reloaded.
        /// </summary>
        public DateTime Last_reload_time { get; set; }

    }
}
