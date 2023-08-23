using Microsoft.Extensions.Hosting;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware.Default
{
    public class WebSocketExternalMediaProvider : BackgroundService, IExternalMediaProvider
    {
        private ConnectionState _connectionState = ConnectionState.None;
        private int _port;
        private UdpClient _socket;
        private Task _executingTask;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private readonly bool _swapByteOrder;
        
        public ConnectionState State => _connectionState;

        public event AudioReceivedHandler OnAudioReceivedHandler;
        public event EndpointConnectedHandler OnEndpointConnectedHandler;

        public WebSocketExternalMediaProvider(int port, bool swapByteOrder = false)
        {
            _port = port;
            _socket = new UdpClient(port);
            _socket.Client.ReceiveBufferSize = 8000;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _swapByteOrder = swapByteOrder;
        }

        public Task Connect()
        {
            _executingTask = ExecuteAsync(_cancellationToken);
            
            if (_executingTask.IsCompleted)
            {
                Task.FromException(new Exception("Connection terminated early"));
            }
            return _executingTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await _socket.ReceiveAsync(stoppingToken);

                if (data.Buffer.Length > 0)
                {
                    var rtpData = data.Buffer;
                    var version = rtpData[0] >> 6;
                    var padded = ((rtpData[0] >> 5) & 0x01) == 1;
                    var extensionHeader = ((rtpData[0] >> 4) & 0x01) == 1;
                    var contributorCount = (rtpData[0] >> 0) & 0x0F;
                    var endOfStream = ((rtpData[1] >> 7) & 0x01) == 1;
                    var payloadType = (rtpData[1] >> 0) & 0x7F;
                    var sequenceNumber = ((uint)rtpData[2] << 8) + (uint)(rtpData[3]);
                    var timeStamp = (uint)(rtpData[4] << 24) + ((uint)rtpData[5] << 16) + (uint)(rtpData[6] << 8) + (uint)rtpData[7];
                    var syncSourceId = (uint)(rtpData[8] << 24) + (uint)(rtpData[9] << 16) + (uint)(rtpData[10] << 8) + (uint)rtpData[11];

                    var payloadStartByte = 4 // V,P,M,SEQ
                                + 4 // time stamp
                                + 4 // Sync Source
                                + (4 * contributorCount);

                    var strippedAudioData = rtpData.Skip(payloadStartByte).ToArray();

                    await OnAudioReceivedHandler(this, strippedAudioData);
                }
            }
        }
    }
}
