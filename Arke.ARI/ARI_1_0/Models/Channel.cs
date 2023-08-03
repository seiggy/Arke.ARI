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
    /// A specific communication connection between Asterisk and an Endpoint.
    /// </summary>
    public class Channel
    {


        /// <summary>
        /// Unique identifier of the channel.  This is the same as the Uniqueid field in AMI.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Protocol id from underlying channel driver (i.e. Call-ID for chan_sip/chan_pjsip; will be empty if not applicable or not implemented by driver).
        /// </summary>
        public string Protocol_id { get; set; }

        /// <summary>
        /// Name of the channel (i.e. SIP/foo-0000a7e3)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public CallerID Caller { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public CallerID Connected { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public string Accountcode { get; set; }

        /// <summary>
        /// Current location in the dialplan
        /// </summary>
        public DialplanCEP Dialplan { get; set; }

        /// <summary>
        /// Timestamp when channel was created
        /// </summary>
        public DateTime Creationtime { get; set; }

        /// <summary>
        /// The default spoken language
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Channel variables
        /// </summary>
        public object Channelvars { get; set; }

    }
}
