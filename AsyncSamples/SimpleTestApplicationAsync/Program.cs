using System;
using System.Threading.Tasks;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arke.ARI.SimpleTestApplicationAsync
{
    internal class Program
    {
        public static AriClient ActionClient;

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
                // Create a new Ari Connection
                ActionClient = new AriClient(
                    new StasisEndpoint("192.168.1.132", 8088, "arke", "arke"),
                    serviceProvider,
                    "arke");

                // Hook into required events
                ActionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                ActionClient.OnChannelDtmfReceivedEvent += ActionClientOnChannelDtmfReceivedEvent;
                ActionClient.OnConnectionStateChanged += ActionClientOnConnectionStateChanged;

                ActionClient.Connect();

                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        private static async Task ActionClientOnConnectionStateChanged(object sender)
        {
            Console.WriteLine("Connection state is now {0}", ActionClient.Connected);
        }

        private static async Task ActionClientOnChannelDtmfReceivedEvent(IAriClient sender, ChannelDtmfReceivedEvent e)
        {
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

        private static async Task c_OnStasisStartEvent(IAriClient sender, StasisStartEvent e)
        {
            // Answer the channel
            await sender.Channels.AnswerAsync(e.Channel.Id);

            // Play an announcement
            await sender.Channels.PlayAsync(e.Channel.Id, "sound:hello-world");
        }
    }
}
