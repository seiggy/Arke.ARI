using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware.Default
{
    public class WebSocketEventProducer : BackgroundService, IEventProducer
    {
        private const int ReceiveChunkSize = 1024;
        private readonly string _application;
        private readonly StasisEndpoint _connectionInfo;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly CancellationToken _cancellationToken;
        private readonly ILogger<WebSocketEventProducer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private ClientWebSocket _client;
        private WebSocketState _lastKnownState;
        private Task _executingTask;

        public event MessageReceivedHandler OnMessageReceived;
        public event ConnectionStateChangedHandler OnConnectionStateChanged;

        #region Constructor

        public WebSocketEventProducer(StasisEndpoint connectionInfo, string application, ILogger<WebSocketEventProducer> logger,
            IServiceProvider serviceProvider)
        {
            _connectionInfo = connectionInfo;
            _application = application;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Public Properties

        public ConnectionState State
        {
            get { return _client == null? ConnectionState.None : (ConnectionState)_client.State; }
        }

        #endregion

        #region Public Methods

        public bool Connected
        {
            get { return _client != null && _client.State == WebSocketState.Open; }
        }

        public async Task ConnectAsync(bool subscribeAll = false, bool ssl = false)
        {
            try
            {
                if (!ssl)
                {
                    _client = new ClientWebSocket();
                    await _client.ConnectAsync(new Uri($"ws://{_connectionInfo.Host}:{_connectionInfo.Port}/ari/events?app={_application}&subscribeAll={subscribeAll}&api_key={$"{_connectionInfo.Username}:{_connectionInfo.Password}"}"), 
                        CancellationToken.None);
                }
                else
                {
                    _client = new ClientWebSocket();
                    await _client.ConnectAsync(new Uri($"wss://{_connectionInfo.Host}:{_connectionInfo.Port}/ari/events?app={_application}&subscribeAll={subscribeAll}&api_key={$"{_connectionInfo.Username}:{_connectionInfo.Password}"}"),
                        CancellationToken.None);
                }
                await CallOnConnected();

                _executingTask = ExecuteAsync(_cancellationToken);
            }
            catch (Exception ex)
            {
                throw new AriException(ex.Message);
            }
        }


        private async Task CallOnMessage(StringBuilder message)
        {
            if (OnMessageReceived != null)
                await OnMessageReceived(this, message.ToString());
        }

        private async Task CallOnDisconnected(string message)
        {
            await OnConnectionStateChanged?.Invoke(this);
        }

        private async Task CallOnConnected()
        {
            if (OnConnectionStateChanged  != null)
                await OnConnectionStateChanged(this);
        }


        public async Task DisconnectAsync()
        {
            if (_client != null)
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Termination Requested", _cancellationToken);
        }

        #endregion

        
        #region Private Methods

        protected virtual void RaiseOnConnectionStateChanged()
        {
            if (_client.State == _lastKnownState) return;

            _lastKnownState = _client.State;
            var handler = OnConnectionStateChanged;
            if (handler != null) handler(this);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await DisconnectAsync();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                while (_client.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                {
                    var stringResult = new StringBuilder();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                            await CallOnDisconnected(null);
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }
                    } while (!result.EndOfMessage);

                    await CallOnMessage(stringResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while listening for websocket messages: {0}", ex);
                await CallOnDisconnected(ex.Message);
            }
            finally
            {
                _client.Dispose();
            }
        }

        #endregion
    }
}
