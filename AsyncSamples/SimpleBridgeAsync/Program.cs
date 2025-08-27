/*
 * SimpleBridge Arke.ARI Bridge Sample
 * Copyright Ben Merrills (ben at mersontech co uk), all rights reserved.
 * https://Arkeari.codeplex.com/
 * https://Arkeari.codeplex.com/license
 *
 * No Warranty. The Software is provided "as is" without warranty of any kind, either express or implied,
 * including without limitation any implied warranties of condition, uninterrupted use, merchantability,
 * fitness for a particular purpose, or non-infringement.
 *
 * Extensions.conf exmaple setup
 *   exten => 7002,1,Noop()
 *   same => n,Stasis(bridge_test)
 *   same => n,hangup()
 *
 */

using Arke.ARI.Dispatchers;
using Arke.ARI.Models;
using Arke.ARI.WebSocket;
using Arke.ARI.WebSocket.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Arke.ARI.SimpleBridgeAsync
{
    class Program
    {
        public static AriClient ActionClient;
        public static Bridge SimpleBridge;
        private static IServiceProvider _serviceProvider;

        private const string AppName = "integration-test";

        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddAriWebSocketWithDispatcher<AsyncTaskDispatcher>((options) =>
            {
                options.ApplicationName = AppName;
                options.BaseUrl = "http://192.168.1.165:8088/ari";
                options.Username = "asterisk";
                options.Password = "asterisk";
            });
            using IHost host = builder.Build();
            _serviceProvider = host.Services;
            await RunDemo(host.Services);
            await host.RunAsync();
        }

        private static async Task RunDemo(IServiceProvider hostProvider)
        {
            var logger = hostProvider.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Starting Simple Bridge Demo Application");

                // Create a message actionClient to receive events on with the new Asterisk environment
                ActionClient = new AriClient(new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk"), hostProvider, AppName);

                ActionClient.EventDispatchingStrategy = EventDispatchingStrategy.AsyncTask;
                ActionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                ActionClient.OnStasisEndEvent += c_OnStasisEndEvent;
                ActionClient.OnChannelDtmfReceivedEvent += c_OnDtmfReceivedEvent;

                logger.LogInformation("Connecting to Asterisk ARI...");
                await ActionClient.Connect(true);

                // Create simple bridge
                SimpleBridge = await ActionClient.Bridges.CreateAsync("mixing", Guid.NewGuid().ToString(), AppName);
                logger.LogInformation("Created bridge with ID: {BridgeId}", SimpleBridge.Id);

                // subscribe to bridge events
                await ActionClient.Applications.SubscribeAsync(AppName, "bridge:" + SimpleBridge.Id);

                // start MOH on bridge
                await ActionClient.Bridges.StartMohAsync(SimpleBridge.Id, "default");
                logger.LogInformation("Started MOH on bridge");

                logger.LogInformation("Bridge demo running. Press keys to control:");
                logger.LogInformation("1 - Stop MOH, 2 - Start MOH, 3 - Mute all, 4 - Unmute all, * - Exit");

                var done = false;
                while (!done)
                {
                    var lastKey = Console.ReadKey();
                    switch (lastKey.KeyChar.ToString())
                    {
                        case "*":
                            done = true;
                            break;
                        case "1":
                            await ActionClient.Bridges.StopMohAsync(SimpleBridge.Id);
                            logger.LogInformation("Stopped MOH");
                            break;
                        case "2":
                            await ActionClient.Bridges.StartMohAsync(SimpleBridge.Id, "default");
                            logger.LogInformation("Started MOH");
                            break;
                        case "3":
                            // Mute all channels on bridge
                            var bridgeMute = await ActionClient.Bridges.GetAsync(SimpleBridge.Id);
                            foreach (var chan in bridgeMute.Channels)
                                await ActionClient.Channels.MuteAsync(chan, "in");
                            logger.LogInformation("Muted all channels on bridge");
                            break;
                        case "4":
                            // Unmute all channels on bridge
                            var bridgeUnmute = await ActionClient.Bridges.GetAsync(SimpleBridge.Id);
                            foreach (var chan in bridgeUnmute.Channels)
                                await ActionClient.Channels.UnmuteAsync(chan, "in");
                            logger.LogInformation("Unmuted all channels on bridge");
                            break;
                    }
                }

                await ActionClient.Bridges.DestroyAsync(SimpleBridge.Id);
                await ActionClient.Disconnect();
                logger.LogInformation("Bridge demo completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in bridge demo application");
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        private static async void c_OnDtmfReceivedEvent(IAriClient sender, ChannelDtmfReceivedEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("DTMF received: {Digit} on channel {ChannelId}", e.Digit, e.Channel.Id);

            switch (e.Digit)
            {
                case "*":
                    break;
                case "1":
                    await ActionClient.Bridges.StopMohAsync(SimpleBridge.Id);
                    break;
                case "2":
                    await ActionClient.Bridges.StartMohAsync(SimpleBridge.Id, "default");
                    break;
                case "3":
                    // Mute all channels on bridge
                    var bridgeMute = await ActionClient.Bridges.GetAsync(SimpleBridge.Id);
                    foreach (var chan in bridgeMute.Channels)
                        await ActionClient.Channels.MuteAsync(chan, "in");
                    break;
                case "4":
                    // Unmute all channels on bridge
                    var bridgeUnmute = await ActionClient.Bridges.GetAsync(SimpleBridge.Id);
                    foreach (var chan in bridgeUnmute.Channels)
                        await ActionClient.Channels.UnmuteAsync(chan, "in");
                    break;
            }
        }

        static async void c_OnStasisEndEvent(object sender, Arke.ARI.Models.StasisEndEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis end event for channel {ChannelId}", e.Channel.Id);

            // remove from bridge
            try
            {
                await ActionClient.Bridges.RemoveChannelAsync(SimpleBridge.Id, e.Channel.Id);
                // hangup
                await ActionClient.Channels.HangupAsync(e.Channel.Id, "normal");
            }
            catch (AriException ex)
            {
                logger.LogError(ex, "Error handling stasis end event");
                Console.WriteLine(ex.ToString());
            }
        }

        static async void c_OnStasisStartEvent(object sender, Arke.ARI.Models.StasisStartEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis start event for channel {ChannelId}", e.Channel.Id);

            // answer channel
            await ActionClient.Channels.AnswerAsync(e.Channel.Id);

            // add to bridge
            await ActionClient.Bridges.AddChannelAsync(SimpleBridge.Id, e.Channel.Id, "member");
        }
    }
}
