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
    /// Error event sent when required params are missing.
    /// </summary>
    public class MissingParams : Message
    {


        /// <summary>
        /// A list of the missing parameters
        /// </summary>
        public List<string> Params { get; set; }

    }
}
