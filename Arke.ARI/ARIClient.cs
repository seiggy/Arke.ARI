﻿using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Arke.ARI.Actions;
using Arke.ARI.Dispatchers;
using Arke.ARI.Middleware;
using Arke.ARI.Middleware.Default;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Arke.ARI
{
    public enum EventDispatchingStrategy
    {
        // Note that dispatching events on the thread pool implies that events might be processed out of order.
        ThreadPool,
        DedicatedThread,
        AsyncTask
    }

    /// <summary>
    /// </summary>
    public class AriClient : BaseAriClient, IDisposable, IAriClient
    {
        public const EventDispatchingStrategy DefaultEventDispatchingStrategy = EventDispatchingStrategy.ThreadPool;

        public delegate Task ConnectionStateChangedHandler(object sender);

        #region Events

        public event ConnectionStateChangedHandler OnConnectionStateChanged;

        #endregion

        #region Private Fields

        private readonly IActionConsumer _actionConsumer;
        private readonly IEventProducer _eventProducer;
        private readonly IServiceProvider _serviceProvider;

        private readonly object _syncRoot = new object();
        private readonly bool _subscribeAllEvents;
        private readonly bool _ssl;
        private bool _autoReconnect;
        private TimeSpan _autoReconnectDelay;
        private IAriDispatcher _dispatcher;
        private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        #endregion

        #region Public Properties

        public IAsteriskActions Asterisk { get; set; }
        public IApplicationsActions Applications { get; set; }
        public IBridgesActions Bridges { get; set; }
        public IChannelsActions Channels { get; set; }
        public IDeviceStatesActions DeviceStates { get; set; }
        public IEndpointsActions Endpoints { get; set; }
        public IEventsActions Events { get; set; }
        public IMailboxesActions Mailboxes { get; set; }
        public IPlaybacksActions Playbacks { get; set; }
        public IRecordingsActions Recordings { get; set; }
        public ISoundsActions Sounds { get; set; }

        public ConnectionState ConnectionState
        {
            get { return _eventProducer.State; }
        }

        public EventDispatchingStrategy EventDispatchingStrategy { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="serviceProvider">An instance of IServiceProvider for DI. Used for getting ILogger instances, and IHttpClientFactory.</param>
        /// <param name="application"></param>
        /// <param name="subscribeAllEvents">Subscribe to all Asterisk events. If provided, the applications listed will be subscribed to all events, effectively disabling the application specific subscriptions.</param>
        /// <param name="ssl">Enable SSL/TLS support for ARI connection</param>
        public AriClient(
            StasisEndpoint endPoint,
            IServiceProvider serviceProvider,
            string application,
            bool subscribeAllEvents = false,
            bool ssl = false)
            // Use Default Middleware
            : this(new RestActionConsumer(endPoint, serviceProvider), 
                  new WebSocketEventProducer(endPoint, application, serviceProvider.GetRequiredService<ILogger<WebSocketEventProducer>>(), serviceProvider), 
                  application, subscribeAllEvents, ssl)
        {
            _serviceProvider = serviceProvider;
        }

        public AriClient(
            IActionConsumer actionConsumer,
            IEventProducer eventProducer,
            string application,
            bool subscribeAllEvents = false,
            bool ssl = false)
        {
            _actionConsumer = actionConsumer;
            _eventProducer = eventProducer;
            EventDispatchingStrategy = DefaultEventDispatchingStrategy;

            // Setup Action Properties
            Asterisk = new AsteriskActions(_actionConsumer);
            Applications = new ApplicationsActions(_actionConsumer);
            Bridges = new BridgesActions(_actionConsumer);
            Channels = new ChannelsActions(_actionConsumer);
            DeviceStates = new DeviceStatesActions(_actionConsumer);
            Endpoints = new EndpointsActions(_actionConsumer);
            Events = new EventsActions(_actionConsumer);
            Mailboxes = new MailboxesActions(_actionConsumer);
            Playbacks = new PlaybacksActions(_actionConsumer);
            Recordings = new RecordingsActions(_actionConsumer);
            Sounds = new SoundsActions(_actionConsumer);
            // Setup Event Handlers
            _eventProducer.OnMessageReceived += _eventProducer_OnMessageReceived;
            _eventProducer.OnConnectionStateChanged += _eventProducer_OnConnectionStateChanged;

            _subscribeAllEvents = subscribeAllEvents;
            _ssl = ssl;
            _serializerOptions.Converters.Add(new DateTimeConverterUsingDateTimeParse());
        }

        public void Dispose()
        {
            _eventProducer.OnConnectionStateChanged -= _eventProducer_OnConnectionStateChanged;
            _eventProducer.OnMessageReceived -= _eventProducer_OnMessageReceived;

            Disconnect().Wait();
        }

        #endregion

        #region Private and Protected Methods


        private async Task _eventProducer_OnConnectionStateChanged(IEventProducer sender)
        {
            if (_eventProducer.State != ConnectionState.Open)
                await Reconnect();

            if (OnConnectionStateChanged != null)
                await OnConnectionStateChanged(sender);
        }

        private async Task _eventProducer_OnMessageReceived(IEventProducer sender, string message)
        {
#if DEBUG
            Debug.WriteLine(message);
#endif
            // load the message
            var jsonMsg = JsonDocument.Parse(message);
            var eventName = jsonMsg.RootElement.GetProperty("type").GetString();
            var type = Type.GetType("Arke.ARI.Models." + eventName + "Event");
            var evnt =
                (type != null)
                    ? (Event)JsonSerializer.Deserialize(message, type, _serializerOptions)
                    : (Event)JsonSerializer.Deserialize(message, typeof(Event), _serializerOptions);

            lock (_syncRoot)
            {
                if (_dispatcher == null)
                    return;

                _dispatcher.QueueAction(() =>
                {
                    try
                    {
                        FireEvent(evnt.Type, evnt, this);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that were thrown by the invoked event handler
                        if (!UnhandledException(this, ex))
                        {
                            Console.WriteLine("The event listener " + evnt.Type.ToString() + " cause an exeption: " + ex.Message);
                        }
                    }
                });
            }
        }

        private async Task Reconnect()
        {
            TimeSpan reconnectDelay;

            lock (_syncRoot)
            {
                var shouldReconnect = _autoReconnect
                    && _eventProducer.State != ConnectionState.Open
                    && _eventProducer.State != ConnectionState.Connecting;

                if (!shouldReconnect)
                    return;

                reconnectDelay = _autoReconnectDelay;
            }

            if (reconnectDelay != TimeSpan.Zero)
                Thread.Sleep(reconnectDelay);
            await _eventProducer.ConnectAsync(_subscribeAllEvents, _ssl);
        }



        IAriDispatcher CreateDispatcher()
        {
            switch (EventDispatchingStrategy)
            {
                case EventDispatchingStrategy.DedicatedThread: return new DedicatedThreadDispatcher();
                case EventDispatchingStrategy.ThreadPool: return new ThreadPoolDispatcher();
                case EventDispatchingStrategy.AsyncTask: return new AsyncDispatcher();
            }

            throw new AriException(EventDispatchingStrategy.ToString());
        }

        #endregion

        #region Public Methods

        public bool Connected
        {
            get { return _eventProducer.State == ConnectionState.Open; }
        }

        public virtual async Task Connect(bool autoReconnect = true, int autoReconnectDelay = 5)
        {
            lock (_syncRoot)
            {
                _autoReconnect = autoReconnect;
                _autoReconnectDelay = TimeSpan.FromSeconds(autoReconnectDelay);
                if (_dispatcher == null)
                    _dispatcher = CreateDispatcher();
            }

            await _eventProducer.ConnectAsync(_subscribeAllEvents, _ssl);
        }

        public virtual async Task Disconnect()
        {
            lock (_syncRoot)
            {
                _autoReconnect = false;
                if (_dispatcher != null)
                {
                    _dispatcher.Dispose();
                    _dispatcher = null;
                }
            }

            await _eventProducer.DisconnectAsync();
        }

        #endregion
    }
}