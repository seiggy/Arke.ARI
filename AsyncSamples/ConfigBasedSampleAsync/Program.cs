using System.Text.Json;
using System.Threading;
using Arke.ARI;
using Arke.ARI.Models;
using Arke.ARI.Models.Events;
using Arke.ARI.WebSocket;
using Arke.ARI.WebSocket.Dispatchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Events = Arke.ARI.Events;

namespace ConfigBasedSampleAsync;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("ConfigBasedSampleAsync - Starting...");
        Console.WriteLine("Press Ctrl+C to exit");
        
        var host = CreateHostBuilder(args).Build();
        var exitEvent = new ManualResetEvent(false);
        
        // Register CTRL+C handler
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            exitEvent.Set();
        };
        
        // Start the application
        await host.StartAsync();
        
        // Get the ARIClient from the service provider
        var ariClient = host.Services.GetRequiredService<IAriClient>();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        
        // Set up the application
        await SetupApplicationAsync(ariClient, logger);
        
        // Wait for exit
        exitEvent.WaitOne();
        
        // Clean shutdown
        logger.LogInformation("Application shutting down...");
        await host.StopAsync();
        
        Console.WriteLine("Application exited.");
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                config.AddEnvironmentVariables();
                
                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Configure ARI Options from appsettings.json
                services.Configure<ARIClientOptions>(hostContext.Configuration.GetSection("ARI"));
                
                // Add HTTP client
                services.AddHttpClient("ARIClient");
                
                // Register event producer
                services.AddSingleton<IEventProducer>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<ARIClientOptions>>();
                    var dispatcher = new AsyncTaskDispatcher();
                    return new WebSocketEventProducer(options.Value, dispatcher);
                });
                
                // Register ARI client
                services.AddSingleton<IAriClient, ARIClient>();
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
    
    private static async Task SetupApplicationAsync(IAriClient ariClient, ILogger logger)
    {
        try
        {
            // Make a simple API call to verify connectivity
            var asteriskInfo = await ariClient.Asterisk.GetInfoAsync();
            logger.LogInformation("Connected to Asterisk {Version} on {Hostname}", 
                asteriskInfo.System.Version, asteriskInfo.System.Hostname);
            
            // Register event handlers
            ariClient.OnStasisStartEvent += (sender, e) => OnStasisStartEvent(sender, e, ariClient, logger);
            ariClient.OnChannelDtmfReceivedEvent += (sender, e) => OnChannelDtmfReceivedEvent(sender, e, ariClient, logger);
            ariClient.OnStasisEndEvent += (sender, e) => 
            {
                logger.LogInformation("Channel {ChannelId} has left the application", e.Channel.Id);
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error connecting to ARI server: {Message}", ex.Message);
            throw;
        }
    }
    
    private static void OnStasisStartEvent(object? sender, StasisStartEvent e, IAriClient ariClient, ILogger logger)
    {
        logger.LogInformation("Channel {ChannelId} has entered the application", e.Channel.Id);
        
        // Answer the channel
        ariClient.Channels.Answer(e.Channel.Id);
        
        // Play welcome message
        ariClient.Channels.Play(
            channelId: e.Channel.Id,
            media: "sound:hello-world");
            
        logger.LogInformation("Answered channel {ChannelId} and played greeting", e.Channel.Id);
    }
    
    private static void OnChannelDtmfReceivedEvent(object? sender, ChannelDtmfReceivedEvent e, IAriClient ariClient, ILogger logger)
    {
        var digit = e.Digit;
        logger.LogInformation("Received DTMF {Digit} on channel {ChannelId}", digit, e.Channel.Id);
        
        switch (digit)
        {
            case "1":
                ariClient.Channels.Play(
                    channelId: e.Channel.Id,
                    media: "sound:digits/1");
                break;
            case "2":
                ariClient.Channels.Play(
                    channelId: e.Channel.Id,
                    media: "sound:digits/2");
                break;
            case "3":
                ariClient.Channels.Play(
                    channelId: e.Channel.Id,
                    media: "sound:digits/3");
                break;
            case "#":
                logger.LogInformation("Processing hangup request for channel {ChannelId}", e.Channel.Id);
                ariClient.Channels.Play(
                    channelId: e.Channel.Id,
                    media: "sound:goodbye");
                
                // Hang up after goodbye sound
                ariClient.Channels.Hangup(e.Channel.Id);
                break;
            default:
                ariClient.Channels.Play(
                    channelId: e.Channel.Id,
                    media: "sound:digits/" + digit);
                break;
        }
    }
} 