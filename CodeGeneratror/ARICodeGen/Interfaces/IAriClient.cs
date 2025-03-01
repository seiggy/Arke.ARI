using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arke.ARI.Actions;
using Arke.ARI.Models;
using Arke.ARI.WebSocket;

namespace Arke.ARI
{
    /// <summary>
    /// Interface for ARI client.
    /// </summary>
    public interface IAriClient : IDisposable
    {
        /// <summary>
        /// Gets the Asterisk actions.
        /// </summary>
        IAsteriskActions Asterisk { get; }

        /// <summary>
        /// Gets the Applications actions.
        /// </summary>
        IApplicationsActions Applications { get; }

        /// <summary>
        /// Gets the Bridges actions.
        /// </summary>
        IBridgesActions Bridges { get; }

        /// <summary>
        /// Gets the Channels actions.
        /// </summary>
        IChannelsActions Channels { get; }

        /// <summary>
        /// Gets the DeviceStates actions.
        /// </summary>
        IDeviceStatesActions DeviceStates { get; }

        /// <summary>
        /// Gets the Endpoints actions.
        /// </summary>
        IEndpointsActions Endpoints { get; }

        /// <summary>
        /// Gets the Events actions.
        /// </summary>
        IEventsActions Events { get; }

        /// <summary>
        /// Gets the Mailboxes actions.
        /// </summary>
        IMailboxesActions Mailboxes { get; }

        /// <summary>
        /// Gets the Playbacks actions.
        /// </summary>
        IPlaybacksActions Playbacks { get; }

        /// <summary>
        /// Gets the Recordings actions.
        /// </summary>
        IRecordingsActions Recordings { get; }

        /// <summary>
        /// Gets the Sounds actions.
        /// </summary>
        ISoundsActions Sounds { get; }

        /// <summary>
        /// Gets a value indicating whether the client is connected.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Connects to the ARI server.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="autoReconnect">Whether to automatically reconnect.</param>
        /// <param name="reconnectDelay">The delay in milliseconds before reconnecting.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ConnectAsync(string applicationName, bool autoReconnect = true, int reconnectDelay = 5000);

        /// <summary>
        /// Disconnects from the ARI server.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DisconnectAsync();

        /// <summary>
        /// Registers an event handler for all events.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        void RegisterEventHandler(IAriEventHandler handler);

        /// <summary>
        /// Registers an event handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="handler">The handler to register.</param>
        void RegisterEventHandler(string eventType, IAriEventHandler handler);

        /// <summary>
        /// Registers a typed event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The handler to register.</param>
        void RegisterEventHandler<T>(IAriEventHandler<T> handler) where T : Event;

        /// <summary>
        /// Unregisters an event handler for all events.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        void UnregisterEventHandler(IAriEventHandler handler);

        /// <summary>
        /// Unregisters an event handler for a specific event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <param name="handler">The handler to unregister.</param>
        void UnregisterEventHandler(string eventType, IAriEventHandler handler);

        /// <summary>
        /// Unregisters a typed event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The handler to unregister.</param>
        void UnregisterEventHandler<T>(IAriEventHandler<T> handler) where T : Event;

        /// <summary>
        /// Sends a GET request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The response.</returns>
        Task<T> GetAsync<T>(string path, Dictionary<string, string> parameters = null);

        /// <summary>
        /// Sends a POST request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="body">The body.</param>
        /// <returns>The response.</returns>
        Task<T> PostAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null);

        /// <summary>
        /// Sends a PUT request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="body">The body.</param>
        /// <returns>The response.</returns>
        Task<T> PutAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null);

        /// <summary>
        /// Sends a DELETE request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The response.</returns>
        Task<T> DeleteAsync<T>(string path, Dictionary<string, string> parameters = null);
    }
} 