using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Arke.ARI.Models;
using Arke.ARI.WebSocket.Dispatchers;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// WebSocket implementation of the event producer.
    /// </summary>
    public class WebSocketEventProducer : IEventProducer, IDisposable
    {
        private readonly ARIClientOptions _options;
        private readonly IDispatcher _dispatcher;
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;
        private ConnectionState _currentState = ConnectionState.Closed;

        /// <summary>
        /// Event raised when a WebSocket event is received.
        /// </summary>
        public event EventHandler<EventReceivedEventArgs> OnEvent;

        /// <summary>
        /// Event raised when the WebSocket connection state changes.
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> OnConnectionStateChanged;

        /// <summary>
        /// Gets a value indicating whether the WebSocket is connected.
        /// </summary>
        public bool IsConnected => _currentState == ConnectionState.Open;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketEventProducer"/> class.
        /// </summary>
        /// <param name="options">The ARI client options.</param>
        /// <param name="dispatcher">The event dispatcher.</param>
        public WebSocketEventProducer(ARIClientOptions options, IDispatcher dispatcher)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        /// <summary>
        /// Connects to the WebSocket.
        /// </summary>
        /// <param name="url">The WebSocket URL.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ConnectAsync(string url)
        {
            if (_currentState == ConnectionState.Open || _currentState == ConnectionState.Connecting)
            {
                await DisconnectAsync();
            }

            SetState(ConnectionState.Connecting);

            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await _webSocket.ConnectAsync(new Uri(url), _cancellationTokenSource.Token);
                SetState(ConnectionState.Open);
                
                // Start receiving messages
                _ = Task.Run(() => ReceiveMessagesAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                SetState(ConnectionState.Failed, ex);
                throw;
            }
        }

        /// <summary>
        /// Disconnects from the WebSocket.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DisconnectAsync()
        {
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
                    // Close the WebSocket gracefully
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
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

        private void SetState(ConnectionState newState, Exception exception = null)
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
                    do
                    {
                        result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            messageBuilder.Append(message);
                        }
                    }
                    while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Text && messageBuilder.Length > 0)
                    {
                        var message = messageBuilder.ToString();
                        messageBuilder.Clear();

                        // Process the message on the dispatcher
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
                // Handle WebSocket exceptions or cancellation
                if (!cancellationToken.IsCancellationRequested)
                {
                    SetState(ConnectionState.Failed, ex);
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                SetState(ConnectionState.Failed, ex);
            }
        }

        private void ProcessMessage(string message)
        {
            try
            {
                // Deserialize the message to an Event object
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var eventData = JsonSerializer.Deserialize<Event>(message, options);
                
                if (eventData != null)
                {
                    // Raise the event
                    OnEvent?.Invoke(this, new EventReceivedEventArgs(eventData, message));
                }
            }
            catch (JsonException ex)
            {
                // Log deserialization errors
                Console.WriteLine($"Error deserializing message: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes the WebSocketEventProducer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the WebSocketEventProducer.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }

            _disposed = true;
        }
    }
} 