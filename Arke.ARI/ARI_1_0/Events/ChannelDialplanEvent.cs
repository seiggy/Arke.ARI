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
    /// Channel changed location in the dialplan.
    /// </summary>
    public class ChannelDialplanEvent : Event
    {


        /// <summary>
        /// The channel that changed dialplan location.
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// The application about to be executed.
        /// </summary>
        public string Dialplan_app { get; set; }

        /// <summary>
        /// The data to be passed to the application.
        /// </summary>
        public string Dialplan_app_data { get; set; }

    }
}
