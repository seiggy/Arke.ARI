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
    /// A text message was received from an endpoint.
    /// </summary>
    public class TextMessageReceivedEvent : Event
    {


        /// <summary>
        /// no description provided
        /// </summary>
        public TextMessage Message { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public Endpoint Endpoint { get; set; }

    }
}
