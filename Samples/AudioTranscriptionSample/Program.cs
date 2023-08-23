using Arke.ARI;
using Arke.ARI.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Arke.ARI.Middleware.Default;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Reflection;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Net;
using Microsoft.CognitiveServices.Speech.Transcription;
using System.Text;
using System.ComponentModel;
using NAudio.Wave;
using NAudio.Codecs;
using AudioTranscriptionSample.Rev.AI;

namespace AudioTranscriptionSample
{
    internal class Program
    {
        public static AriClient ActionClient;
        public static Bridge SimpleBridge;
        static string speechKey;
        static string speechRegion;
        static int listenPort;
        static ConversationTranscriber speechRecognizer;
        static PushAudioInputStream pushAudioInputStream;
        //static BinaryAudioStreamReader audioStreamReader;
        
        static TaskCompletionSource<int> stopRecognition;

        static RevAiClient revAiClient;

        private const string AppName = "arke";

        static async Task Main(string[] args)
        {
            try
            {
                stopRecognition = new TaskCompletionSource<int>();
                HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
                builder.Services.AddLogging();
                builder.Services.AddHttpClient();
                var configBuilder = new ConfigurationBuilder();
                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                configBuilder.AddUserSecrets(Assembly.GetExecutingAssembly());
                configBuilder.AddEnvironmentVariables();
                builder.Configuration.AddConfiguration(configBuilder.Build());
                using IHost host = builder.Build();
                await RunDemo(host.Services);
                await host.RunAsync();
                Task.WaitAny(new[] { stopRecognition.Task });
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                
            }
        }

        private static async Task RunDemo(IServiceProvider serviceProvider)
        {
            try
            {
                pushAudioInputStream = new PushAudioInputStream(AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1));
                //audioStreamReader = new BinaryAudioStreamReader(new MemoryStream());
                

                var config = serviceProvider.GetService<IConfiguration>();
                speechKey = config?.GetValue<string>("SpeechKey") ?? string.Empty;
                speechRegion = config?.GetValue<string>("SpeechRegion") ?? string.Empty;
                listenPort = config?.GetValue<int>("listenPort") ?? 5555;

                var revAiKey = config?.GetValue<string>("RevAiKey") ?? string.Empty;
                revAiClient = new RevAiClient(revAiKey);
                revAiClient.OnPartialResponseReceived += RevAiClient_OnPartialResponseReceived;
                revAiClient.OnFinalResponseReceived += RevAiClient_OnFinalResponseReceived;

                if (listenPort == 0) listenPort = 23356;
                ActionClient = new AriClient(new StasisEndpoint("192.168.1.132", 8088, AppName, "arke"), serviceProvider, AppName);

                ActionClient.EventDispatchingStrategy = EventDispatchingStrategy.AsyncTask;
                ActionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                ActionClient.OnStasisEndEvent += c_OnStasisEndEvent;
                
                await ActionClient.Connect();

                // Create simple bridge
                SimpleBridge = await ActionClient.Bridges.CreateAsync("mixing", Guid.NewGuid().ToString(), AppName);

                // subscribe to bridge events
                await ActionClient.Applications.SubscribeAsync(AppName, "bridge:" + SimpleBridge.Id);

                var externalMediaProvider = new WebSocketExternalMediaProvider(listenPort);
                externalMediaProvider.OnAudioReceivedHandler += ExternalMediaProvider_OnAudioReceivedHandler;
                Task.Run(() => externalMediaProvider.Connect());
                //var externalMediaChannel = await ActionClient.Channels.ExternalMediaAsync()

                var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
                speechConfig.SetProfanity(ProfanityOption.Raw);
                speechRecognizer = new ConversationTranscriber(speechConfig, AudioConfig.FromStreamInput(pushAudioInputStream));
                speechRecognizer.Transcribing += SpeechRecognizer_Transcribing;
                speechRecognizer.Transcribed += SpeechRecognizer_Recognized;
                speechRecognizer.Canceled += SpeechRecognizer_Canceled;
                speechRecognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("STOPPING Transcription");
                    stopRecognition.TrySetResult(0);
                };
                
                var externalChannel = await ActionClient.Channels.ExternalMediaAsync("arke", $"{await GetLocalIpAddress()}:{listenPort}", "slin16", encapsulation: "rtp");
                
                //await ActionClient.Bridges.StartMohAsync(SimpleBridge.Id, "default");
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static async Task RevAiClient_OnFinalResponseReceived(RevAiClient sender, RevAiResponse response)
        {
            if (response.Transcript == null) return;
            var text = string.Concat(response.Transcript.Select(t => t.Value));
            Console.WriteLine($"RESPONSE: {response.StartTimeStamp}: {text}");
        }

        private static async Task RevAiClient_OnPartialResponseReceived(RevAiClient sender, RevAiResponse response)
        {
            // do nothing
        }

        private static void SpeechRecognizer_Canceled(object? sender, ConversationTranscriptionCanceledEventArgs e)
        {
            Console.WriteLine($"CANCELLED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
            }
            stopRecognition.TrySetResult(0);
        }

        private static void SpeechRecognizer_Transcribing(object? sender, ConversationTranscriptionEventArgs e)
        {
            //Console.Write($"TRANSCRIBING: {e.Result.Text}");
        }

        static async Task<string> GetLocalIpAddress()
        {
            var host = await Dns.GetHostEntryAsync(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return ip.MapToIPv4().ToString();
            }
            throw new Exception("No IP Available");
        }

        private static void SpeechRecognizer_Recognized(object? sender, ConversationTranscriptionEventArgs e)
        {
            switch (e.Result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    Console.WriteLine($"RECOGNIZED: Text={e.Result.Text} Speaker ID={e.Result.SpeakerId}");
                    break;
                case ResultReason.NoMatch:
                    Console.WriteLine("NOMATCH: Unrecognizable audio");
                    break;
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(e.Result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetail={cancellation.ErrorDetails}");
                        Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                    }
                    break;
            }
        }

        static bool transcribing = false;
        
        private static async Task ExternalMediaProvider_OnAudioReceivedHandler(Arke.ARI.Middleware.IExternalMediaProvider sender, byte[] audio)
        {
            //pushAudioInputStream.Write(audio);
            if (!transcribing) return;

            // swap from big endian to little endian
            var swappedAudio = new byte[audio.Length];
            for (int i = 0; i < swappedAudio.Length; i+=2)
            {
                swappedAudio[i] = audio[i+1];
                swappedAudio[(i+1)] = audio[i];
            }

            //pushAudioInputStream.Write(swappedAudio);
            await revAiClient.WriteAudioToSocket(swappedAudio);
            //var samples = new byte[audio.Length * 2];
            //var samplePos = 0;
            //for (int i = 0; i < audio.Length; i++)
            //{
            //    short pcm = MuLawDecoder.MuLawToLinearSample(audio[i]);
            //    samples[samplePos] = (byte)(pcm & 0xFF);
            //    samplePos++;
            //    samples[samplePos] = (byte)(pcm >> 8);
            //    samplePos++;
            //}
            //waveProvider.AddSamples(samples, 0, samples.Length);

            //await audioStreamReader.CopyToStream(audio);
            //rawDataStream.Write(audio, 0, audio.Length);
            //var result = await speechRecognizer.RecognizeOnceAsync();
            //Console.WriteLine($"RECONGIZED: Text={result.Text}");
        }


        static async Task c_OnStasisEndEvent(object sender, Arke.ARI.Models.StasisEndEvent e)
        {
            // remove from bridge
            try
            {
                await ActionClient.Bridges.RemoveChannelAsync(SimpleBridge.Id, e.Channel.Id);
            }
            catch (AriException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                await ActionClient.Channels.HangupAsync(e.Channel.Id, "normal");
            }
            catch (AriException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            try
            {
                await ActionClient.Recordings.StopAsync("sample_audio");
            }
            catch (AriException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            //await speechRecognizer.StopTranscribingAsync().ConfigureAwait(false);
            transcribing = false;
            await revAiClient.EndStream();
        }

        static async Task c_OnStasisStartEvent(object sender, Arke.ARI.Models.StasisStartEvent e)
        {
            // answer channel
            await ActionClient.Channels.AnswerAsync(e.Channel.Id);

            // add to bridge
            await ActionClient.Bridges.AddChannelAsync(SimpleBridge.Id, e.Channel.Id, "member", mute: false);

            if (e.Channel.Caller.Number == "5000")
            {
                //await Task.Delay(TimeSpan.FromSeconds(10));
                //await speechRecognizer.StartTranscribingAsync().ConfigureAwait(false);
                Console.WriteLine("Audio transcription start!");
                transcribing = true;
                await revAiClient.ConnectAsync();
            }
        }
    }
}