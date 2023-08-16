using System;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            await RunDemo(host.Services);
            await host.RunAsync();
        }

        private static async Task RunDemo(IServiceProvider serviceProvider)
        { 
            try
            {
                // Create a message actionClient to receive events on
                actionClient = new AriClient(new StasisEndpoint("192.168.1.132", 8088, "arke", "arke"), serviceProvider, "arke");

                actionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                actionClient.OnStasisEndEvent += c_OnStasisEndEvent;
                actionClient.OnRecordingFinishedEvent += ActionClientOnRecordingFinishedEvent;

                actionClient.Connect();

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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        static async Task GetRecording(Channel c)
        {
            var playback = await actionClient.Channels.PlayAsync(c.Id, "sound:vm-rec-name", "en", 0, 0, Guid.NewGuid().ToString());
            recording = new RecordingToChannel()
            {
                Recording = await actionClient.Channels.RecordAsync(c.Id, "temp-recording", "wav", 6, 1, "overwrite", true, "#"),
                Channel = c
            };
        }

        static async Task PlaybackRecording(Channel c)
        {
            var repeat = await actionClient.Channels.PlayAsync(c.Id, "recording:temp-recording", "en", 0, 0, Guid.NewGuid().ToString());
        }

        static async Task ActionClientOnRecordingFinishedEvent(object sender, Arke.ARI.Models.RecordingFinishedEvent e)
        {
            if (e.Recording.Name != recording.Recording.Name) return;

            await PlaybackRecording(recording.Channel);

            await GetRecording(recording.Channel);
        }

        static async Task c_OnStasisEndEvent(object sender, Arke.ARI.Models.StasisEndEvent e)
        {
            // Delete recording
            await actionClient.Recordings.DeleteStoredAsync("temp-recording");

            // hangup
            await actionClient.Channels.HangupAsync(e.Channel.Id, "normal");
        }

        static async Task c_OnStasisStartEvent(object sender, Arke.ARI.Models.StasisStartEvent e)
        {
            // answer channel
            await actionClient.Channels.AnswerAsync(e.Channel.Id);

            await GetRecording(e.Channel);
        }
    }
}
