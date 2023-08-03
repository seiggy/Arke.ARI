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
    /// Asterisk ping information
    /// </summary>
    public class AsteriskPing
    {


        /// <summary>
        /// Asterisk id info
        /// </summary>
        public string Asterisk_id { get; set; }

        /// <summary>
        /// Always string value is pong
        /// </summary>
        public string Ping { get; set; }

        /// <summary>
        /// The timestamp string of request received time
        /// </summary>
        public string Timestamp { get; set; }

    }
}
