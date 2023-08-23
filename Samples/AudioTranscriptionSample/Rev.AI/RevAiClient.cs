using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AudioTranscriptionSample.Rev.AI
{
    public delegate Task ResponseReceivedHandler(RevAiClient sender, RevAiResponse response);

    public class RevAiResponse
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("ts")]
        public double? StartTimeStamp { get; set; }

        [JsonPropertyName("end_ts")]
        public double? EndTimeStamp { get; set; }

        [JsonPropertyName("elements")]
        public List<RevAiElement>? Transcript { get; set; }
    }

    public class RevAiElement
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("ts")]
        public double? StartTimeStamp { get; set; }

        [JsonPropertyName("end_ts")]
        public double? EndTimeStamp { get; set; }

        [JsonPropertyName("confidence")]
        public double? ConfidenceScore { get; set; }
    }

    public class RevAiClient
    {
        private readonly string _accessKey;
        private ClientWebSocket _client;
        private const string REV_AI_ENDPOINT = "wss://api.rev.ai/speechtotext/v1/stream";
        private CancellationTokenSource _tokenSource;
        private const int ReceiveBufferSize = 2048;
        
        public event ResponseReceivedHandler? OnPartialResponseReceived;
        public event ResponseReceivedHandler? OnFinalResponseReceived;

        public RevAiClient(string accessKey)
        {
            _accessKey = accessKey;
        }

        public async Task ConnectAsync()
        {
            if (_client != null)
            {
                if (_client.State == WebSocketState.Open) return;
                else _client.Dispose();
            }
            _client = new ClientWebSocket();

            if (_tokenSource != null) _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();

            try
            {
                await _client.ConnectAsync(new Uri($"{REV_AI_ENDPOINT}?access_token={_accessKey}&content_type=audio/x-raw;layout=interleaved;rate=16000;format=S16LE;channels=1&start_ts=10&transcriber=machine_v2&priority=speed"), _tokenSource.Token);
                Task.Factory.StartNew(ReceiveLoop, _tokenSource.Token, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting to RevAI: {0}", e);
            }
        }

        private async Task ReceiveLoop()
        {
            var loopToken = _tokenSource.Token;
            MemoryStream outputStream = null;
            WebSocketReceiveResult receiveResult = null;
            var buffer = new byte[ReceiveBufferSize];

            try
            {
                while (!loopToken.IsCancellationRequested)
                {
                    outputStream = new MemoryStream(ReceiveBufferSize);

                    do
                    {
                        receiveResult = await _client.ReceiveAsync(buffer, _tokenSource.Token).ConfigureAwait(false);
                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                            outputStream.Write(buffer, 0, receiveResult.Count);
                    } while (!receiveResult.EndOfMessage);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Socket Terminated: {0}", receiveResult.CloseStatusDescription);
                        break;
                    }

                    outputStream.Position = 0;
                    await ResponseReceived(outputStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while receiving data from socket: {0}", e);
            }
            finally
            {
                outputStream?.Dispose();
            }
        }

        private async Task ResponseReceived(MemoryStream outputStream)
        {
            var data = UTF8Encoding.UTF8.GetString(outputStream.ToArray());
            Console.WriteLine("RESPONSE: {0}", data);
            var response = JsonSerializer.Deserialize<RevAiResponse>(data);
            if (response == null)
            {
                return;
            }

            if (response.Type == "partial" && OnPartialResponseReceived != null)
                await OnPartialResponseReceived(this, response);
            else if (response.Type == "final" && OnFinalResponseReceived != null)
                await OnFinalResponseReceived(this, response);
        }

        MemoryStream outgoingAudio = new MemoryStream();

        public async Task EndStream()
        {
            if (_client != null && _client.State == WebSocketState.Open)
                await _client.SendAsync(Encoding.UTF8.GetBytes("EOS"), WebSocketMessageType.Text, true, _tokenSource.Token);
        }

        public async Task WriteAudioToSocket(byte[] audio)
        {
            if (_client != null && _client.State == WebSocketState.Open)
            {
                outgoingAudio.Write(audio);
                if (outgoingAudio.Position >= 16000)
                {
                    var outputBuffer = outgoingAudio.ToArray();
                    await _client.SendAsync(outputBuffer, WebSocketMessageType.Binary, true, _tokenSource.Token);
                    outgoingAudio.Position = 0;
                }
            }
        }
    }
}
