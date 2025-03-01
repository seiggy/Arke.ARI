using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{
    /// <summary>
    /// Base class for all ARI action classes.
    /// </summary>
    public abstract class ActionBase
    {
        /// <summary>
        /// Gets the ARI client.
        /// </summary>
        protected IAriClient Client { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase"/> class.
        /// </summary>
        /// <param name="client">The ARI client.</param>
        protected ActionBase(IAriClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        /// <summary>
        /// Sends a GET request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The response.</returns>
        protected async Task<T> GetAsync<T>(string path, Dictionary<string, string> parameters = null)
        {
            return await Client.GetAsync<T>(path, parameters);
        }

        /// <summary>
        /// Sends a POST request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="body">The body.</param>
        /// <returns>The response.</returns>
        protected async Task<T> PostAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null)
        {
            return await Client.PostAsync<T>(path, parameters, body);
        }

        /// <summary>
        /// Sends a PUT request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="body">The body.</param>
        /// <returns>The response.</returns>
        protected async Task<T> PutAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null)
        {
            return await Client.PutAsync<T>(path, parameters, body);
        }

        /// <summary>
        /// Sends a DELETE request to the ARI server.
        /// </summary>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <param name="path">The path.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The response.</returns>
        protected async Task<T> DeleteAsync<T>(string path, Dictionary<string, string> parameters = null)
        {
            return await Client.DeleteAsync<T>(path, parameters);
        }
    }

    public class AsteriskActions : ActionBase, IAsteriskActions
    {
        public AsteriskActions(ARIClient client) : base(client) { }
    }

    public class ApplicationsActions : ActionBase, IApplicationsActions
    {
        public ApplicationsActions(ARIClient client) : base(client) { }
    }

    public class BridgesActions : ActionBase, IBridgesActions
    {
        public BridgesActions(ARIClient client) : base(client) { }
    }

    public class ChannelsActions : ActionBase, IChannelsActions
    {
        public ChannelsActions(ARIClient client) : base(client) { }
    }

    public class DeviceStatesActions : ActionBase, IDeviceStatesActions
    {
        public DeviceStatesActions(ARIClient client) : base(client) { }
    }

    public class EndpointsActions : ActionBase, IEndpointsActions
    {
        public EndpointsActions(ARIClient client) : base(client) { }
    }

    public class EventsActions : ActionBase, IEventsActions
    {
        public EventsActions(ARIClient client) : base(client) { }
    }

    public class MailboxesActions : ActionBase, IMailboxesActions
    {
        public MailboxesActions(ARIClient client) : base(client) { }
    }

    public class PlaybacksActions : ActionBase, IPlaybacksActions
    {
        public PlaybacksActions(ARIClient client) : base(client) { }
    }

    public class RecordingsActions : ActionBase, IRecordingsActions
    {
        public RecordingsActions(ARIClient client) : base(client) { }
    }

    public class SoundsActions : ActionBase, ISoundsActions
    {
        public SoundsActions(ARIClient client) : base(client) { }
    }
} 