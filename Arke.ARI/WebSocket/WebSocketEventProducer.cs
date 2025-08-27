using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using Arke.ARI.WebSocket.Dispatchers;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;
using System.Collections.Generic;
using System.Linq;

namespace Arke.ARI.WebSocket
{
    public class WebSocketEventProducer : IEventProducer, IDisposable
    {
        public const string ActivitySourceName = "Arke.ARI.WebSocket";
        private readonly ARIClientOptions _options;
        private readonly IDispatcher _dispatcher;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _disposed;
        private ConnectionState _currentState = ConnectionState.Closed;
        private static readonly ActivitySource ActivitySource = new ActivitySource(ActivitySourceName);

        public event EventHandler<EventReceivedEventArgs>? OnEvent;
        public event EventHandler<ConnectionStateChangedEventArgs>? OnConnectionStateChanged;
        public bool IsConnected => _currentState == ConnectionState.Open;
        private readonly ILogger<WebSocketEventProducer> _logger;

        public WebSocketEventProducer(
            ARIClientOptions options,
            IDispatcher dispatcher,
            ILogger<WebSocketEventProducer> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public async Task ConnectAsync(string url)
        {
            using var activity = ActivitySource.StartActivity(nameof(ConnectAsync), ActivityKind.Client);

            activity?.SetTag("network.protocol.name", "websocket");
            activity?.SetTag("url.full", url);
            if (_currentState == ConnectionState.Open || _currentState == ConnectionState.Connecting)
            {
                await DisconnectAsync();
            }

            SetState(ConnectionState.Connecting);
            _webSocket = new ClientWebSocket();

            if (activity?.Id != null)
            {
                _webSocket.Options.SetRequestHeader("traceparent", activity.Id);
            }

            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var uri = new Uri(url);
                activity?.SetTag("server.address", uri.Host);
                if (!uri.IsDefaultPort) activity?.SetTag("server.port", uri.Port);
                await _webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);
                SetState(ConnectionState.Open);
                activity?.SetStatus(ActivityStatusCode.Ok);
                
                _ = Task.Run(() => ReceiveMessagesAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "Error connecting to WebSocket at {Url}", url);
                SetState(ConnectionState.Failed, ex);
                throw;
            }
        }

        public async Task DisconnectAsync()
        {
            using var activity = ActivitySource.StartActivity(nameof(DisconnectAsync), ActivityKind.Client);
            activity?.SetTag("network.protocol.name", "websocket");
            if (_currentState == ConnectionState.Closed || _currentState == ConnectionState.Closing)
            {
                return;
            }

            SetState(ConnectionState.Closing);

            try
            {
                _cancellationTokenSource?.Cancel();
                if (_webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "Error disconnecting from WebSocket");
                SetState(ConnectionState.Failed, ex);
            }
            finally
            {
                _webSocket?.Dispose();
                _webSocket = null;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
                SetState(ConnectionState.Closed);
            }
        }

        private void SetState(ConnectionState newState, Exception? exception = null)
        {
            var previousState = _currentState;
            _currentState = newState;
            OnConnectionStateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(previousState, newState, exception));
        }

        private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new byte[_options.ReceiveBufferSize];
            var messageBuilder = new StringBuilder();
            try
            {
                while (!cancellationToken.IsCancellationRequested && _webSocket != null && _webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    using var receiveActivity = ActivitySource.StartActivity(nameof(ReceiveMessagesAsync), ActivityKind.Consumer);
                    receiveActivity?.SetTag("messaging.system", "websocket");
                    receiveActivity?.SetTag("network.protocol.name", "websocket");

                    do
                    {
                        result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            messageBuilder.Append(message);
                        }
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Text && messageBuilder.Length > 0)
                    {
                        var message = messageBuilder.ToString();
                        messageBuilder.Clear();
                        receiveActivity?.SetTag("message.size", message.Length);
                        using var messageActivity = ActivitySource.StartActivity("websocket.message", ActivityKind.Consumer);
                        messageActivity.SetParentId(receiveActivity?.Id);
                        messageActivity?.SetTag("messaging.system", "websocket");
                        messageActivity?.SetTag("message.size", message.Length);
                        await _dispatcher.DispatchAsync(() => ProcessMessage(message));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await DisconnectAsync();
                        break;
                    }
                }
            }
            catch (Exception ex) when (ex is WebSocketException || ex is OperationCanceledException)
            {
                Activity.Current?.AddException(ex);
                Activity.Current?.SetStatus(ActivityStatusCode.Error, ex.Message);
                _logger.LogError(ex, "WebSocket receive loop terminated unexpectedly");
                if (!cancellationToken.IsCancellationRequested)
                {
                    SetState(ConnectionState.Failed, ex);
                }
            }
            catch (Exception ex)
            {
                Activity.Current?.AddException(ex);
                Activity.Current?.SetStatus(ActivityStatusCode.Error,ex.Message);
                _logger.LogError(ex, "Unexpected error in WebSocket receive loop");
                SetState(ConnectionState.Failed, ex);
            }
        }

        private void ProcessMessage(string message)
        {
            using var activity = ActivitySource.StartActivity("ari.event.process", ActivityKind.Internal);
            activity?.SetTag("message.size", message?.Length ?? 0);
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var eventData = JsonSerializer.Deserialize<Event>(message, options);
                if (eventData != null)
                {
                    activity?.SetTag("ari.application", eventData.Application);
                    activity?.SetTag("ari.timestamp", eventData.Timestamp);
                    OnEvent?.Invoke(this, new EventReceivedEventArgs(eventData, message));
                }
            }
            catch (JsonException e)
            {
                activity?.AddException(e);
                activity?.SetStatus(ActivityStatusCode.Error, e.Message);
                _logger.LogError(e, "Error deserializing WebSocket message: {Message}", message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }
            _disposed = true;
        }
    }
}
