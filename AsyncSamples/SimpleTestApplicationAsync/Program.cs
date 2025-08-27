using System;
using System.Threading.Tasks;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Arke.ARI.SimpleTestApplicationAsync
{
    internal class Program
    {
        public static AriClient ActionClient;
        private static IServiceProvider _serviceProvider;

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
                logger.LogInformation("Starting ARI Demo Application");

                // Create a new Ari Connection with the new Asterisk environment
                ActionClient = new AriClient(
                    new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk"),
                    serviceProvider,
                    "arke");

                // Hook into required events
                ActionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                ActionClient.OnChannelDtmfReceivedEvent += ActionClientOnChannelDtmfReceivedEvent;
                
                logger.LogInformation("Connecting to Asterisk ARI...");
                await ActionClient.Connect(true);

                logger.LogInformation("Demo application running. Press any key to exit.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in demo application");
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        private static async void ActionClientOnConnectionStateChanged(object sender)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Connection state is now {Connected}", ActionClient.Connected);
        }

        private static async void ActionClientOnChannelDtmfReceivedEvent(IAriClient sender, ChannelDtmfReceivedEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("DTMF received: {Digit} on channel {ChannelId}", e.Digit, e.Channel.Id);

            // When DTMF received
            switch (e.Digit)
            {
                case "*":
                    await sender.Channels.PlayAsync(e.Channel.Id, "sound:asterisk-friend");
                    break;
                case "#":
                    await sender.Channels.PlayAsync(e.Channel.Id, "sound:goodbye");
                    await sender.Channels.HangupAsync(e.Channel.Id, "normal");
                    break;
                default:
                    await sender.Channels.PlayAsync(e.Channel.Id, string.Format("sound:digits/{0}", e.Digit));
                    break;
            }
        }

        private static async void c_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis start event received for channel {ChannelId}", e.Channel.Id);

            // Answer the channel
            await sender.Channels.AnswerAsync(e.Channel.Id);

            // Play an announcement
            await sender.Channels.PlayAsync(e.Channel.Id, "sound:hello-world");
        }
    }
}
