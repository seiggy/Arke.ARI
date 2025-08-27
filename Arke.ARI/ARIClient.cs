using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Arke.ARI.Actions;
using Arke.ARI.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Arke.ARI.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Arke.ARI.Dispatchers;

namespace Arke.ARI
{
    public class AriClient : BaseAriClient, IAriClient, IDisposable, IAsyncDisposable
    {
        private readonly HttpClient _http;
        private readonly IEventProducer _eventProducer;
        private readonly object _syncRoot = new object();
        private bool _disposed;
        private readonly bool _subscribeAllEvents;
        private readonly string _application;
        private IAriDispatcher? _dispatcher;
        private ILogger? _logger;
        private bool _ssl;
        private StasisEndpoint _endPoint;

        public AriClient(StasisEndpoint endPoint, IServiceProvider serviceProvider, string application)
            : this(endPoint, serviceProvider, application, false, false)
        {
        }

        public AriClient(StasisEndpoint endPoint, IServiceProvider serviceProvider, string application, bool subscribeAllEvents, bool ssl)
        {
            if (endPoint == null) throw new ArgumentNullException(nameof(endPoint));
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (string.IsNullOrWhiteSpace(application)) throw new ArgumentException("application", nameof(application));
            _application = application;

            var scheme = ssl ? "https" : "http";
            _ssl = ssl;
            _endPoint = endPoint;
            _http = new HttpClient { BaseAddress = new Uri($"{scheme}://{endPoint.Host}:{endPoint.Port}/ari/") };
            var auth = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{endPoint.Username}:{endPoint.Password}"));
            _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
            _subscribeAllEvents = subscribeAllEvents;
            _dispatcher = CreateDispatcher();
            _eventProducer = serviceProvider.GetRequiredService<IEventProducer>();
            _logger = serviceProvider.GetService<ILogger<AriClient>>();
            _eventProducer.OnConnectionStateChanged += _eventProducer_OnConnectionStateChanged;
            _eventProducer.OnEvent += _eventProducer_OnMessageReceived;

            Asterisk = new AsteriskActions(this);
            Applications = new ApplicationsActions(this);
            Bridges = new BridgesActions(this);
            Channels = new ChannelsActions(this);
            DeviceStates = new DeviceStatesActions(this);
            Endpoints = new EndpointsActions(this);
            Events = new EventsActions(this);
            Mailboxes = new MailboxesActions(this);
            Playbacks = new PlaybacksActions(this);
            Recordings = new RecordingsActions(this);
            Sounds = new SoundsActions(this);
        }

        public IAsteriskActions Asterisk { get; }
        public IApplicationsActions Applications { get; }
        public IBridgesActions Bridges { get; }
        public IChannelsActions Channels { get; }
        public IDeviceStatesActions DeviceStates { get; }
        public IEndpointsActions Endpoints { get; }
        public IEventsActions Events { get; }
        public IMailboxesActions Mailboxes { get; }
        public IPlaybacksActions Playbacks { get; }
        public IRecordingsActions Recordings { get; }
        public ISoundsActions Sounds { get; }

        public bool Connected => _eventProducer.IsConnected;

        public EventDispatchingStrategy EventDispatchingStrategy { get; set; } = EventDispatchingStrategy.ThreadPool;

        public async Task Connect(bool subscribeAllEvents)
        {
            // need to setup and connect the websocket engine.
            lock (_syncRoot)
            {
                if (_dispatcher == null)
                    _dispatcher = CreateDispatcher();
            }
            var websocketEndpoint = $"{(_ssl ? "wss" : "ws")}://{_endPoint.Host}:{_endPoint.Port}/ari/events?api_key={_endPoint.Username}:{_endPoint.Password}&app={_application}&subscribeAllEvents={subscribeAllEvents}";
            await _eventProducer.ConnectAsync(websocketEndpoint);
        }

        IAriDispatcher CreateDispatcher()
        {
            switch (EventDispatchingStrategy)
            {
                case EventDispatchingStrategy.ThreadPool:
                    return new ThreadPoolDispatcher();
                case EventDispatchingStrategy.AsyncTask:
                    return new AsyncDispatcher();
                case EventDispatchingStrategy.DedicatedThread:
                    return new DedicatedThreadDispatcher();
                default:
                    throw new InvalidOperationException("Unknown EventDispatchingStrategy: " + EventDispatchingStrategy);
            }
        }

        public Task Disconnect()
        {
            return Task.CompletedTask;
        }

        private static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (path.StartsWith("/")) path = path.Substring(1);
            return path;
        }
        public async Task<T> GetAsync<T>(string path) => await _http.GetFromJsonAsync<T>(Normalize(path)) ?? default!;
        public async Task GetAsync(string path) { var r = await _http.GetAsync(Normalize(path)); r.EnsureSuccessStatusCode(); }
        public async Task<T> PostAsync<T>(string path) { var r = await _http.PostAsync(Normalize(path), null); r.EnsureSuccessStatusCode(); return await r.Content.ReadFromJsonAsync<T>() ?? default!; }
        public async Task PostAsync(string path) { var r = await _http.PostAsync(Normalize(path), null); r.EnsureSuccessStatusCode(); }
        public async Task<T> PutAsync<T>(string path) { var r = await _http.PutAsync(Normalize(path), null); r.EnsureSuccessStatusCode(); return await r.Content.ReadFromJsonAsync<T>() ?? default!; }
        public async Task PutAsync(string path) { var r = await _http.PutAsync(Normalize(path), null); r.EnsureSuccessStatusCode(); }
        public async Task DeleteAsync(string path) { var r = await _http.DeleteAsync(Normalize(path)); r.EnsureSuccessStatusCode(); }
        public async Task<T> DeleteAsync<T>(string path) { var r = await _http.DeleteAsync(Normalize(path)); r.EnsureSuccessStatusCode(); return await r.Content.ReadFromJsonAsync<T>() ?? default!; }

        public void Dispose()
        {
            if (_disposed) return;
            _http.Dispose();
            _disposed = true;
        }

        public ValueTask DisposeAsync()
        {
            Dispose();
            return ValueTask.CompletedTask;
        }

        private void _eventProducer_OnConnectionStateChanged(object? sender, EventArgs e)
        {

        }

        private void _eventProducer_OnMessageReceived(object sender, EventReceivedEventArgs e)
        {
#if DEBUG
            Debug.WriteLine(e.RawData);
#endif
            var eventName = e.EventData.Type;
            var type = Type.GetType("Arke.ARI.Models." + eventName + "Event");

            var evnt =
                type != null
                ? (Event)JsonSerializer.Deserialize(e.RawData, type)
                : (Event)JsonSerializer.Deserialize(e.RawData, typeof(Event));

            lock (_syncRoot)
            {
                if (_dispatcher == null) return;

                _dispatcher.QueueAction(() =>
                {
                    try
                    {
                        FireEvent(evnt.Type, evnt, this);
                    }
                    catch (Exception e)
                    {
                        if (!UnhandledException(this, e))
                            _logger?.LogError(e, "Error processing event {EventType}", evnt.Type);
                    }
                });
            }
        }
    }
}
