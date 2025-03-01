using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Arke.ARI;
using Arke.ARI.WebSocket;
using Arke.ARI.WebSocket.Dispatchers;

namespace Arke.ARI.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring and registering the ARI client with dependency injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds ARI client services to the specified <see cref="IServiceCollection"/>, binding options from configuration.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The configuration section that contains the ARIClient settings.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddARIClient(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.Configure<ARIClientOptions>(configuration);
            
            return AddARIClientCore(services);
        }

        /// <summary>
        /// Adds ARI client services to the specified <see cref="IServiceCollection"/>, configuring options with the provided action.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ARIClientOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddARIClient(this IServiceCollection services, Action<ARIClientOptions> configureOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            
            return AddARIClientCore(services);
        }

        /// <summary>
        /// Adds ARI client services to the specified <see cref="IServiceCollection"/> with the option to configure
        /// HTTP client behavior.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ARIClientOptions"/>.</param>
        /// <param name="configureClient">A delegate to configure the underlying HttpClient.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddARIClient(
            this IServiceCollection services, 
            Action<ARIClientOptions> configureOptions,
            Action<IHttpClientBuilder> configureClient)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            
            // Get the client name from options to ensure consistency
            var options = new ARIClientOptions();
            configureOptions(options);
            string clientName = options.HttpClientName;

            // Configure the named HttpClient
            var clientBuilder = services.AddHttpClient(clientName);
            configureClient?.Invoke(clientBuilder);
            
            return AddARIClientCore(services);
        }

        /// <summary>
        /// Adds ARI client services to the specified <see cref="IServiceCollection"/> with a custom WebSocket dispatcher.
        /// </summary>
        /// <typeparam name="TDispatcher">The type of the dispatcher to use for WebSocket events.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configureOptions">A delegate to configure the <see cref="ARIClientOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddARIClientWithDispatcher<TDispatcher>(
            this IServiceCollection services, 
            Action<ARIClientOptions> configureOptions)
            where TDispatcher : class, IDispatcher
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configureOptions == null) throw new ArgumentNullException(nameof(configureOptions));

            services.Configure(configureOptions);
            services.TryAddSingleton<IDispatcher, TDispatcher>();
            
            return AddARIClientCore(services);
        }

        private static IServiceCollection AddARIClientCore(IServiceCollection services)
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
            
            // Register the ARI client as a singleton with the interface
            services.TryAddSingleton<IAriClient, ARIClient>();
            
            return services;
        }
    }
} 