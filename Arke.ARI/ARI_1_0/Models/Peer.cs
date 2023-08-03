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
    /// Detailed information about a remote peer that communicates with Asterisk.
    /// </summary>
    public class Peer
    {


        /// <summary>
        /// The current state of the peer. Note that the values of the status are dependent on the underlying peer technology.
        /// </summary>
        public string Peer_status { get; set; }

        /// <summary>
        /// An optional reason associated with the change in peer_status.
        /// </summary>
        public string Cause { get; set; }

        /// <summary>
        /// The IP address of the peer.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The port of the peer.
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// The last known time the peer was contacted.
        /// </summary>
        public string Time { get; set; }

    }
}
