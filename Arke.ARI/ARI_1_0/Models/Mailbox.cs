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
    /// Represents the state of a mailbox.
    /// </summary>
    public class Mailbox
    {


        /// <summary>
        /// Name of the mailbox.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Count of old messages in the mailbox.
        /// </summary>
        public int Old_messages { get; set; }

        /// <summary>
        /// Count of new messages in the mailbox.
        /// </summary>
        public int New_messages { get; set; }

    }
}
