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
    /// Event showing the start of a recording operation.
    /// </summary>
    public class RecordingStartedEvent : Event
    {


        /// <summary>
        /// Recording control object
        /// </summary>
        public LiveRecording Recording { get; set; }

    }
}
