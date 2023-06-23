/*
   AsterNET ARI Framework
   Automatically generated file @ 6/21/2023 2:39:12 PM
*/
using System;
using System.Collections.Generic;
using AsterNET.ARI.Actions;

namespace AsterNET.ARI.Models
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
