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
    /// Info about how Asterisk was built
    /// </summary>
    public class BuildInfo
    {


        /// <summary>
        /// OS Asterisk was built on.
        /// </summary>
        public string Os { get; set; }

        /// <summary>
        /// Kernel version Asterisk was built on.
        /// </summary>
        public string Kernel { get; set; }

        /// <summary>
        /// Compile time options, or empty string if default.
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// Machine architecture (x86_64, i686, ppc, etc.)
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// Date and time when Asterisk was built.
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Username that build Asterisk
        /// </summary>
        public string User { get; set; }

    }
}
