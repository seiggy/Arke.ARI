using System;
using System.Diagnostics;
using Arke.ARI;
using Arke.ARI.Models;
using SimpleConfAsync.REST;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SimpleConfAsync
{
    public class AppConfig
    {
        public const string AppName = "simpleconf";
        public const string RestAddress = "http://localhost:9000/";
    }

    internal class Program
    {
        public static AriClient Client;
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
                logger.LogInformation("Starting Simple Conference Demo Application");
                
                Client = new AriClient(
                    new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk"), 
                    serviceProvider,
                    AppConfig.AppName);

                Conference.Conferences.Add(new Conference(Client, Guid.NewGuid(), "test"));

                Client.OnStasisStartEvent += c_OnStasisStartEvent;
                Client.OnStasisEndEvent += c_OnStasisEndEvent;

                logger.LogInformation("Connecting to Asterisk ARI...");
                Client.Connect();

                // Start REST
                WebApp.Start<Startup>(url: AppConfig.RestAddress);
                logger.LogInformation("Conference demo loaded and waiting for connections at {RestAddress}", AppConfig.RestAddress);

                // Wait
                Console.ReadKey();

                // Destroy all the conferences and their bridges
                Conference.Conferences.ForEach(async x => await x.DestroyConference());
                Conference.Conferences = null;
                
                logger.LogInformation("Conference demo completed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in conference demo application");
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        private static async void c_OnStasisEndEvent(object sender, StasisEndEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis end event for channel {ChannelId}", e.Channel.Id);
            
            if (e.Application != AppConfig.AppName) return;

            var conf = Conference.Conferences.SingleOrDefault(x => x.ConferenceUsers.Any(c => c.Channel.Id == e.Channel.Id));
            if (conf == null) return;

            await conf.RemoveUser(e.Channel.Id);
        }

        private static async void c_OnStasisStartEvent(object sender, StasisStartEvent e)
        {
            var logger = _serviceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Stasis start event for channel {ChannelId}", e.Channel.Id);
            
            if (e.Application != AppConfig.AppName) return;
            var failed = true;
            if (e.Args.Count == 0)
                await Client.Channels.SetChannelVarAsync(e.Channel.Id, "CONFEXIT", "NOTFOUND");

            var confId = e.Args[0];
            var conf = Conference.Conferences.SingleOrDefault(x => x.ConferenceName == confId);
            if (conf == null)
                await Client.Channels.SetChannelVarAsync(e.Channel.Id, "CONFEXIT", "NOTFOUND");
            else
                if (!await conf.AddUser(e.Channel))
                await Client.Channels.SetChannelVarAsync(e.Channel.Id, "CONFEXIT", "CANTJOIN");
            else
            {
                logger.LogInformation("Added channel {ChannelId} to conference {ConferenceId}", e.Channel.Id, confId);
                failed = false;
            }

            if (failed)
                await Client.Channels.ContinueInDialplanAsync(e.Channel.Id,
                    e.Channel.Dialplan.Context,
                    e.Channel.Dialplan.Exten,
                    (int)e.Channel.Dialplan.Priority++);
        }
    }
}
