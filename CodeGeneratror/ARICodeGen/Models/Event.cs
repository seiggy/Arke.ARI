using System;
using System.Text.Json.Serialization;

namespace Arke.ARI.Models
{
    /// <summary>
    /// Base class for all ARI events.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The type of event.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The time at which this event was created.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Name of the application receiving the event.
        /// </summary>
        [JsonPropertyName("application")]
        public string Application { get; set; } = string.Empty;
    }
} 