﻿/*
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
 
using Arke.ARI.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Arke.ARI.SimpleBridgeAsync
{
    class Program
    {
        public static AriClient ActionClient;
        public static Bridge SimpleBridge;

        private const string AppName = "arke";

        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            using IHost host = builder.Build();
            await RunDemo(host.Services);
            await host.RunAsync();
        }

        private static async Task RunDemo(IServiceProvider hostProvider)
        {
            try
            {
                // Create a message actionClient to receive events on
                ActionClient = new AriClient(new StasisEndpoint("192.168.1.132", 8088, AppName, "arke"), hostProvider, AppName);

                ActionClient.EventDispatchingStrategy = EventDispatchingStrategy.AsyncTask;
                ActionClient.OnStasisStartEvent += c_OnStasisStartEvent;
                ActionClient.OnStasisEndEvent += c_OnStasisEndEvent;
                ActionClient.OnChannelDtmfReceivedEvent += c_OnDtmfReceivedEvent;

                ActionClient.Connect();

                // Create simple bridge
                SimpleBridge = await ActionClient.Bridges.CreateAsync("mixing", Guid.NewGuid().ToString(), AppName);

                // subscribe to bridge events
                await ActionClient.Applications.SubscribeAsync(AppName, "bridge:" + SimpleBridge.Id);

                // start MOH on bridge
                await ActionClient.Bridges.StartMohAsync(SimpleBridge.Id, "default");

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

                await ActionClient.Bridges.DestroyAsync(SimpleBridge.Id);
                ActionClient.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        private static async void c_OnDtmfReceivedEvent(IAriClient sender, ChannelDtmfReceivedEvent e)
        {
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
            // remove from bridge
            try
            {
                await ActionClient.Bridges.RemoveChannelAsync(SimpleBridge.Id, e.Channel.Id);
                // hangup
                await ActionClient.Channels.HangupAsync(e.Channel.Id, "normal");
            }
            catch (AriException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static async void c_OnStasisStartEvent(object sender, Arke.ARI.Models.StasisStartEvent e)
        {
            // answer channel
            await ActionClient.Channels.AnswerAsync(e.Channel.Id);

            // add to bridge
            await ActionClient.Bridges.AddChannelAsync(SimpleBridge.Id, e.Channel.Id, "member");
        }
    }
}
