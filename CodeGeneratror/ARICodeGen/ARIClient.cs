using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Arke.ARI.Actions;
using Arke.ARI.Models;
using Arke.ARI.WebSocket;
using Arke.ARI.WebSocket.Dispatchers;

[assembly: InternalsVisibleTo("ARICodeGen.Tests")]

namespace Arke.ARI
{
    /// <summary>
    /// Options for configuring the ARI client
    /// </summary>
    public class ARIClientOptions
    {
        /// <summary>
        /// The base URL of the ARI endpoint (e.g., "http://asterisk:8088/ari")
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:8088/ari";
        
        /// <summary>
        /// The username for ARI authentication
        /// </summary>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// The password for ARI authentication
        /// </summary>
        public string Password { get; set; } = string.Empty;
        
        /// <summary>
        /// The HTTP client name to use when creating a client with IHttpClientFactory
        /// </summary>
        public string HttpClientName { get; set; } = "ARIClient";

        /// <summary>
        /// Gets or sets the application name for ARI connection.
        /// </summary>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically reconnect on WebSocket connection failure.
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// Gets or sets the delay in milliseconds before attempting to reconnect a failed WebSocket connection.
        /// </summary>
        public int ReconnectDelay { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the maximum number of reconnect attempts before giving up on a WebSocket connection.
        /// </summary>
        public int MaxReconnectAttempts { get; set; } = 10;

        /// <summary>
        /// Gets or sets the WebSocket connection timeout in milliseconds.
        /// </summary>
        public int ConnectionTimeout { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the WebSocket receive buffer size in bytes.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 8192;

        /// <summary>
        /// Gets or sets a value indicating whether to subscribe to all ARI events.
        /// </summary>
        public bool SubscribeAllEvents { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to use SSL/TLS for the WebSocket connection.
        /// </summary>
        public bool UseSecureWebSocket { get; set; } = false;
    }

    /// <summary>
    /// Main client for ARI API interactions
    /// </summary>
    public class ARIClient : IAriClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ARIClientOptions _options;
        private readonly string _baseUrl;
        private readonly string _username;
        private readonly string _password;
        private readonly IEventProducer _eventProducer;
        private readonly AriEventHandlerRegistry _eventHandlerRegistry = new AriEventHandlerRegistry();
        private bool _connected;
        private bool _disposeHttpClient;
        private bool _disposed;

        public IAsteriskActions Asterisk { get; private set; }
        public IApplicationsActions Applications { get; private set; }
        public IBridgesActions Bridges { get; private set; }
        public IChannelsActions Channels { get; private set; }
        public IDeviceStatesActions DeviceStates { get; private set; }
        public IEndpointsActions Endpoints { get; private set; }
        public IEventsActions Events { get; private set; }
        public IMailboxesActions Mailboxes { get; private set; }
        public IPlaybacksActions Playbacks { get; private set; }
        public IRecordingsActions Recordings { get; private set; }
        public ISoundsActions Sounds { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the client is connected to the ARI server via WebSocket.
        /// </summary>
        public bool Connected => _connected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ARIClient"/> class using dependency injection.
        /// </summary>
        /// <param name="options">The options for configuring the client.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="eventProducer">The WebSocket event producer.</param>
        public ARIClient(IOptions<ARIClientOptions> options, IHttpClientFactory httpClientFactory, IEventProducer eventProducer = null)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            _options = options.Value;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClientFactory.CreateClient(_options.HttpClientName);
            _baseUrl = _options.BaseUrl.TrimEnd('/');
            _username = _options.Username;
            _password = _options.Password;
            _disposeHttpClient = false;

            // Initialize WebSocket-related fields
            if (eventProducer != null)
            {
                _eventProducer = eventProducer;
                _eventProducer.OnEvent += EventProducer_OnEvent;
                _eventProducer.OnConnectionStateChanged += EventProducer_OnConnectionStateChanged;
            }
            else
            {
                // Create a WebSocketEventProducer with default AsyncTaskDispatcher
                // Note: In production, this fallback should rarely be used as the eventProducer should be injected
                var dispatcher = new AsyncTaskDispatcher();
                _eventProducer = new WebSocketEventProducer(_options, dispatcher);
                _eventProducer.OnEvent += EventProducer_OnEvent;
                _eventProducer.OnConnectionStateChanged += EventProducer_OnConnectionStateChanged;
            }

            InitializeActions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ARIClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base URL for the ARI server.</param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        [Obsolete("Use the constructor accepting IOptions<ARIClientOptions> instead.")]
        public ARIClient(string baseUrl, string username, string password)
            : this(baseUrl, username, password, new HttpClient(), true)
        {
        }

        /// <summary>
        /// Creates a new instance of the ARIClient with an existing HttpClient instance
        /// </summary>
        /// <param name="baseUrl">The base URL of the ARI endpoint</param>
        /// <param name="username">The username for authorization</param>
        /// <param name="password">The password for authorization</param>
        /// <param name="httpClient">The HttpClient instance to use</param>
        [Obsolete("Consider using ARIClient with IOptions<ARIClientOptions> and IHttpClientFactory for better resource management")]
        public ARIClient(string baseUrl, string username, string password, HttpClient httpClient)
            : this(baseUrl, username, password, httpClient, false)
        {
        }

        [Obsolete("Use the constructor accepting IOptions<ARIClientOptions> instead.")]
        public ARIClient(string baseUrl, string username, string password, HttpClient httpClient, bool disposeHttpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            _baseUrl = baseUrl.TrimEnd('/');
            _httpClient = httpClient;
            _disposeHttpClient = disposeHttpClient;
            _httpClientFactory = null;
            _username = username;
            _password = password;
            
            // Initialize options for backward compatibility
            _options = new ARIClientOptions
            {
                BaseUrl = baseUrl,
                Username = username,
                Password = password,
                AutoReconnect = true,
                ReconnectDelay = 5000,
                ApplicationName = "default"
            };
            
            _eventProducer = new WebSocketEventProducer(_options, new AsyncTaskDispatcher());
            
            // Subscribe to events from the producer
            _eventProducer.OnEvent += EventProducer_OnEvent;
            _eventProducer.OnConnectionStateChanged += EventProducer_OnConnectionStateChanged;

            InitializeActions();
        }

        private void EventProducer_OnEvent(object sender, EventReceivedEventArgs e)
        {
            if (e.EventData != null)
            {
                _eventHandlerRegistry.DispatchEvent(e.EventData);
            }
        }

        private void EventProducer_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            _connected = e.NewState == Arke.ARI.WebSocket.ConnectionState.Open;
            
            // If auto-reconnect is enabled and we're disconnected, attempt to reconnect
            if (_options.AutoReconnect && e.NewState == Arke.ARI.WebSocket.ConnectionState.Closed && !_disposed)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(_options.ReconnectDelay);
                    if (!_disposed)
                    {
                        try
                        {
                            await ConnectAsync(_options.ApplicationName, _options.AutoReconnect, _options.ReconnectDelay);
                        }
                        catch
                        {
                            // Reconnection failed, will try again later
                        }
                    }
                });
            }
        }

        private void InitializeActions()
        {
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

        public async Task ConnectAsync(string applicationName, bool autoReconnect = true, int reconnectDelay = 5000)
        {
            if (string.IsNullOrEmpty(applicationName))
                throw new ArgumentNullException(nameof(applicationName));

            // Update options
            _options.ApplicationName = applicationName;
            _options.AutoReconnect = autoReconnect;
            _options.ReconnectDelay = reconnectDelay;
            
            // Build the WebSocket URL
            var wsUrl = $"{_baseUrl.Replace("http://", "ws://").Replace("https://", "wss://")}/ari/events?api_key={_username}:{_password}&app={applicationName}";
            
            // Connect to the WebSocket
            await _eventProducer.ConnectAsync(wsUrl);
        }

        public async Task DisconnectAsync()
        {
            await _eventProducer.DisconnectAsync();
            _connected = false;
        }

        // For backward compatibility
        public void Connect(bool autoReconnect = true, int autoReconnectDelay = 5)
        {
            // Use the application name from options or a default
            var applicationName = _options.ApplicationName ?? "default";
            ConnectAsync(applicationName, autoReconnect, autoReconnectDelay * 1000).GetAwaiter().GetResult();
        }

        // For backward compatibility
        public void Disconnect()
        {
            DisconnectAsync().GetAwaiter().GetResult();
        }

        // Event handler registration methods
        public void RegisterEventHandler(IAriEventHandler handler)
        {
            _eventHandlerRegistry.RegisterHandler(handler);
        }

        public void RegisterEventHandler(string eventType, IAriEventHandler handler)
        {
            _eventHandlerRegistry.RegisterHandler(eventType, handler);
        }

        public void RegisterEventHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            _eventHandlerRegistry.RegisterHandler(handler);
        }

        public void UnregisterEventHandler(IAriEventHandler handler)
        {
            _eventHandlerRegistry.UnregisterHandler(handler);
        }

        public void UnregisterEventHandler(string eventType, IAriEventHandler handler)
        {
            _eventHandlerRegistry.UnregisterHandler(eventType, handler);
        }

        public void UnregisterEventHandler<T>(IAriEventHandler<T> handler) where T : Event
        {
            _eventHandlerRegistry.UnregisterHandler(handler);
        }

        // HTTP request methods
        public async Task<T> GetAsync<T>(string path, Dictionary<string, string> parameters = null)
        {
            var response = await SendRequestAsync(HttpMethod.Get, path, parameters);
            return await DeserializeResponseAsync<T>(response);
        }

        public async Task<T> PostAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null)
        {
            var response = await SendRequestAsync(HttpMethod.Post, path, parameters, body);
            return await DeserializeResponseAsync<T>(response);
        }

        public async Task<T> PutAsync<T>(string path, Dictionary<string, string> parameters = null, object body = null)
        {
            var response = await SendRequestAsync(HttpMethod.Put, path, parameters, body);
            return await DeserializeResponseAsync<T>(response);
        }

        public async Task<T> DeleteAsync<T>(string path, Dictionary<string, string> parameters = null)
        {
            var response = await SendRequestAsync(HttpMethod.Delete, path, parameters);
            return await DeserializeResponseAsync<T>(response);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string path, Dictionary<string, string> parameters = null, object body = null)
        {
            var url = $"{_baseUrl}/ari/{path}";
            
            if (parameters != null && parameters.Count > 0)
            {
                var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
                url = $"{url}?{queryString}";
            }

            var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new ARIException((int)response.StatusCode, errorContent);
            }

            return response;
        }

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (string.IsNullOrEmpty(content))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new ARIException(0, $"Failed to deserialize response: {ex.Message}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _disposed = true;
                
                // Disconnect the WebSocket
                _eventProducer.OnEvent -= EventProducer_OnEvent;
                _eventProducer.OnConnectionStateChanged -= EventProducer_OnConnectionStateChanged;
                
                try
                {
                    _eventProducer.DisconnectAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    // Ignore exceptions during disposal
                }
                
                // Dispose the HTTP client if we own it
                if (_disposeHttpClient)
                {
                    _httpClient.Dispose();
                }
            }

            _disposed = true;
        }
    }

    public class ARIException : Exception
    {
        public int StatusCode { get; }

        public ARIException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
} 