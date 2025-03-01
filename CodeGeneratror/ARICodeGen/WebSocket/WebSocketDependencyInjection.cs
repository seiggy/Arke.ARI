using System;
using Arke.ARI.Models;
using Arke.ARI.WebSocket.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Arke.ARI.WebSocket
{
    /// <summary>
    /// Extension methods for adding ARI WebSocket services to the service collection.
    /// </summary>
    public static class WebSocketDependencyInjection
    {
        /// <summary>
        /// Adds the ARI WebSocket services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">A delegate to configure the options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAriWebSocket(
            this IServiceCollection services,
            Action<ARIClientOptions> configureOptions)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
                
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));
                
            services.Configure(configureOptions);
            
            return AddAriWebSocketCore(services);
        }
        
        /// <summary>
        /// Adds the ARI WebSocket services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="options">The ARI client options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAriWebSocket(
            this IServiceCollection services,
            ARIClientOptions options)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
                
            if (options == null)
                throw new ArgumentNullException(nameof(options));
                
            services.TryAddSingleton(Options.Create(options));
            
            return AddAriWebSocketCore(services);
        }

        /// <summary>
        /// Adds the ARI WebSocket services to the service collection with a specific dispatcher.
        /// </summary>
        /// <typeparam name="TDispatcher">The type of the dispatcher to use.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">A delegate to configure the options.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddAriWebSocketWithDispatcher<TDispatcher>(
            this IServiceCollection services,
            Action<ARIClientOptions> configureOptions)
            where TDispatcher : class, IDispatcher
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
                
            if (configureOptions == null)
                throw new ArgumentNullException(nameof(configureOptions));
                
            services.Configure(configureOptions);
            services.TryAddSingleton<IDispatcher, TDispatcher>();
            
            return AddAriWebSocketCore(services);
        }
        
        private static IServiceCollection AddAriWebSocketCore(IServiceCollection services)
        {
            // Register the default dispatcher if none is registered
            services.TryAddSingleton<IDispatcher, AsyncTaskDispatcher>();
            
            // Register the event producer directly as a singleton
            services.TryAddSingleton<IEventProducer>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ARIClientOptions>>().Value;
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                return new WebSocketEventProducer(options, dispatcher);
            });
            
            return services;
        }
    }
} 