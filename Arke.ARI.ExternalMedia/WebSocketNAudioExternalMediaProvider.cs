using Microsoft.Extensions.Hosting;
using NAudio.CoreAudioApi.Interfaces;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware.ExternalMedia
{
    public class WebSocketNAudioExternalMediaProvider : BackgroundService, IExternalMediaProvider
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

        private readonly IAudioClient _audioClient;
        private readonly AudioFormat _audioFormat;
        private readonly IRTPAudioProcessor _rtpAudioProcessor;

        public WebSocketNAudioExternalMediaProvider(int port, AudioFormat audioFormat, bool swapByteOrder = false)
        {
            _port = port;
            _socket = new UdpClient(port);
            _socket.Client.ReceiveBufferSize = 256;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _swapByteOrder = swapByteOrder;
            _audioFormat = audioFormat;
            _rtpAudioProcessor = RTPAudioProcessorFactory.GetAudioProcessorForAudioFormat(audioFormat);
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
                    var chunk = await ReadOneMessage(data.Buffer);
                }
            }
        }

        protected async Task<byte[]> ReadOneMessage(byte[] rtpData)
        {
            var rtpMessage = new RtpMessage();

            rtpMessage.Version = rtpData[0] >> 6;
            rtpMessage.Padded = ((rtpData[0] >> 5) & 0x01) == 1;
            rtpMessage.ExtensionHeader = ((rtpData[0] >> 4) & 0x01) == 1;
            rtpMessage.ContributorCount = (rtpData[0] >> 0) & 0x0F;
            rtpMessage.EndOfStream = ((rtpData[1] >> 7) & 0x01) == 1;
            rtpMessage.PayloadType = (rtpData[1] >> 0) & 0x7F;
            rtpMessage.SequenceNumber = ((uint)rtpData[2] << 8) + (uint)(rtpData[3]);
            rtpMessage.TimeStamp = (uint)(rtpData[4] << 24) + ((uint)rtpData[5] << 16) + (uint)(rtpData[6] << 8) + (uint)rtpData[7];
            rtpMessage.SyncSourceId = (uint)(rtpData[8] << 24) + (uint)(rtpData[9] << 16) + (uint)(rtpData[10] << 8) + (uint)rtpData[11];

            var payloadStartByte = 4 // V,P,M,SEQ
                        + 4 // time stamp
                        + 4 // Sync Source
                        + (4 * rtpMessage.ContributorCount);

            return rtpData.Skip(payloadStartByte).ToArray();
        }
    }

    public class RtpMessage
    {
        public int Version { get; set; }
        public bool Padded { get; set; }
        public bool ExtensionHeader { get; set; }
        public int ContributorCount { get; set; }
        public bool EndOfStream { get; set; }
        public int PayloadType { get; set; }
        public uint SequenceNumber { get; set; }
        public uint TimeStamp { get; set; }
        public uint SyncSourceId { get; set; }
        public int ContributorId { get; set; }
        public byte[] Payload { get; set; }

    }
}
