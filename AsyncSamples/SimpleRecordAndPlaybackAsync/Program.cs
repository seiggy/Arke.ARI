using System;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SimpleRecordAndPlaybackAsync
{
    /*
     * This simple example shows how to queue a playback, initiate a recording
     * and then play back that recording when it's been completed.
     *
     * This example doesn't allow multiple calls, and is pretty much useless for
     * anything else than showing how to use Play and Record.
     *
     * Once the recording has been played back, the process starts again.
     *
     * The recording will wait for a '#' digit, 1 second of silence or cut off
     * after 6 seconds of recording.
     */

    class Program
    {
        public static AriClient actionClient;
        private static IServiceProvider _serviceProvider;

        public static RecordingToChannel recording;

        public class RecordingToChannel
        {
            public LiveRecording Recording { get; set; }
            public Channel Channel { get; set; }
        }

        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            using IHost host = builder.Build();
            _serviceProvider = host.Services;
            await RunDemo(host.Services);
            await host.RunAsync();
        }

        private static async Task RunDemo(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting Record and Playback Demo Application");

                // Create a message actionClient to receive events on with the new Asterisk environment
                actionClient = new AriClient(new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk"), serviceProvider, "arke");

                actionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                actionClient.OnStasisEndEvent += c_OnStasisEndEvent;
                actionClient.OnRecordingFinishedEvent += ActionClientOnRecordingFinishedEvent;

                logger.LogInformation("Connecting to Asterisk ARI...");
                actionClient.Connect();

                logger.LogInformation("Record and Playback demo running. Press * to exit.");

                bool done = false;
                while (!done)
                {
                    var lastKey = Console.ReadKey();
                    switch (lastKey.KeyChar.ToString())
                    {
                        case "*":
                            done = true;
                            break;
                    }
                }

                actionClient.Disconnect();
                logger.LogInformation("Record and Playback demo completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in record and playback demo application");
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        static async Task GetRecording(Channel c)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting recording on channel {ChannelId}", c.Id);

            var playback = await actionClient.Channels.PlayAsync(c.Id, "sound:vm-rec-name", "en", 0, 0, Guid.NewGuid().ToString());
            recording = new RecordingToChannel()
            {
                Recording = await actionClient.Channels.RecordAsync(c.Id, "temp-recording", "wav", 6, 1, "overwrite", true, "#"),
                Channel = c
            };
        }

        static async Task PlaybackRecording(Channel c)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Playing back recording on channel {ChannelId}", c.Id);

            var repeat = await actionClient.Channels.PlayAsync(c.Id, "recording:temp-recording", "en", 0, 0, Guid.NewGuid().ToString());
        }

        static async Task ActionClientOnRecordingFinishedEvent(object sender, Arke.ARI.Models.RecordingFinishedEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Recording finished: {RecordingName}", e.Recording.Name);

            if (e.Recording.Name != recording.Recording.Name) return;

            await PlaybackRecording(recording.Channel);

            await GetRecording(recording.Channel);
        }

        static async Task c_OnStasisEndEvent(object sender, Arke.ARI.Models.StasisEndEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis end event for channel {ChannelId}", e.Channel.Id);

            // Delete recording
            await actionClient.Recordings.DeleteStoredAsync("temp-recording");

            // hangup
            await actionClient.Channels.HangupAsync(e.Channel.Id, "normal");
        }

        static async Task c_OnStasisStartEvent(object sender, Arke.ARI.Models.StasisStartEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis start event for channel {ChannelId}", e.Channel.Id);

            // answer channel
            await actionClient.Channels.AnswerAsync(e.Channel.Id);

            await GetRecording(e.Channel);
        }
    }
}
