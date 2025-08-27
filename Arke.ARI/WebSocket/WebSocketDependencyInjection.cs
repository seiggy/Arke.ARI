using System;
using Arke.ARI.WebSocket.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Arke.ARI.WebSocket
{
    public static class WebSocketDependencyInjection
    {
        public static IServiceCollection AddAriWebSocket(this IServiceCollection services, Action<ARIClientOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));
            services.Configure(configureOptions);
            return AddAriWebSocketCore(services);
        }

        public static IServiceCollection AddAriWebSocket(this IServiceCollection services, ARIClientOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));
            services.TryAddSingleton(Options.Create(options));
            return AddAriWebSocketCore(services);
        }

        public static IServiceCollection AddAriWebSocketWithDispatcher<TDispatcher>(this IServiceCollection services, Action<ARIClientOptions> configureOptions)
            where TDispatcher : class, IDispatcher
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));
            services.Configure(configureOptions);
            services.TryAddSingleton<IDispatcher, TDispatcher>();
            return AddAriWebSocketCore(services);
        }

        private static IServiceCollection AddAriWebSocketCore(IServiceCollection services)
        {
            services.TryAddSingleton<IDispatcher, AsyncTaskDispatcher>();
            services.TryAddSingleton<IEventProducer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ARIClientOptions>>().Value;
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                var logger = sp.GetRequiredService<ILogger<WebSocketEventProducer>>();
                return new WebSocketEventProducer(options, dispatcher, logger);
            });
            return services;
        }
    }
}
