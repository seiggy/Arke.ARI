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
    /// A recording that is in progress
    /// </summary>
    public class LiveRecording
    {


        /// <summary>
        /// Base name for the recording
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Recording format (wav, gsm, etc.)
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// URI for the channel or bridge being recorded
        /// </summary>
        public string Target_uri { get; set; }

        /// <summary>
        /// no description provided
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Duration in seconds of the recording
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Duration of talking, in seconds, detected in the recording. This is only available if the recording was initiated with a non-zero maxSilenceSeconds.
        /// </summary>
        public int Talking_duration { get; set; }

        /// <summary>
        /// Duration of silence, in seconds, detected in the recording. This is only available if the recording was initiated with a non-zero maxSilenceSeconds.
        /// </summary>
        public int Silence_duration { get; set; }

        /// <summary>
        /// Cause for recording failure if failed
        /// </summary>
        public string Cause { get; set; }

    }
}
