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
    /// Notification that another WebSocket has taken over for an application.  An application may only be subscribed to by a single WebSocket at a time. If multiple WebSockets attempt to subscribe to the same application, the newer WebSocket wins, and the older one receives this event.
    /// </summary>
    public class ApplicationReplacedEvent : Event
    {


    }
}
