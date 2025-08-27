using System;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using Arke.ARI.WebSocket;
using Arke.ARI.WebSocket.Dispatchers;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Arke.ARI.Tests
{
    /// <summary>
    /// Base class for all ARI tests providing common setup and utilities
    /// </summary>
    public abstract class TestBase : IAsyncDisposable
    {
        protected readonly ITestOutputHelper Output;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ILogger<TestBase> Logger;
        protected readonly StasisEndpoint TestEndpoint;

        protected TestBase(ITestOutputHelper output)
        {
            Output = output;

            // Setup DI container for testing
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });
            services.AddHttpClient();

            var clientOptions = new ARIClientOptions
            {
                BaseUrl = "http://192.168.1.165:8088/ari",
                Username = "asterisk",
                Password = "asterisk",
                ApplicationName = "integration-test",
                SubscribeAllEvents = true,
                ReconnectDelay = TimeSpan.FromSeconds(5).Milliseconds,
                MaxReconnectAttempts = 5
            };
            services.AddTransient<ARIClientOptions>(_ => clientOptions);
            services.AddTransient<IDispatcher, AsyncTaskDispatcher>();
            services.AddTransient<IEventProducer, WebSocketEventProducer>();

            ServiceProvider = services.BuildServiceProvider();
            Logger = ServiceProvider.GetRequiredService<ILogger<TestBase>>();

            // Use test Asterisk environment
            TestEndpoint = new StasisEndpoint("192.168.1.165", 8088, "asterisk", "asterisk");
        }

        /// <summary>
        /// Creates a test AriClient instance
        /// </summary>
        protected AriClient CreateTestClient(string application = "test-app")
        {
            return new AriClient(TestEndpoint, ServiceProvider, application);
        }

        /// <summary>
        /// Creates a mock service for testing
        /// </summary>
        protected T CreateMockService<T>() where T : class
        {
            return A.Fake<T>();
        }

        /// <summary>
        /// Asserts that an async operation completes successfully
        /// </summary>
        protected async Task AssertAsyncOperation(Func<Task> operation, string operationName = "Operation")
        {
            try
            {
                await operation();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{OperationName} failed", operationName);
                throw;
            }
        }

        /// <summary>
        /// Asserts that an async operation returns a result
        /// </summary>
        protected async Task<T> AssertAsyncOperation<T>(Func<Task<T>> operation, string operationName = "Operation")
        {
            try
            {
                var result = await operation();
                Assert.NotNull(result);
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{OperationName} failed", operationName);
                throw;
            }
        }

        /// <summary>
        /// Creates a test channel for testing
        /// </summary>
        protected Channel CreateTestChannel(string channelId = null)
        {
            return new Channel
            {
                Id = channelId ?? Guid.NewGuid().ToString(),
                Name = $"SIP/test-{Guid.NewGuid():N}",
                State = "Ring",
                Caller = new CallerID { Name = "Test Caller", Number = "1234567890" }
            };
        }

        /// <summary>
        /// Creates a test bridge for testing
        /// </summary>
        protected Bridge CreateTestBridge(string bridgeId = null)
        {
            return new Bridge
            {
                Id = bridgeId ?? Guid.NewGuid().ToString(),
                Creator = "test-app",
                Name = $"test-bridge-{Guid.NewGuid():N}",
                Technology = "simple_bridge"
            };
        }

        public virtual async ValueTask DisposeAsync()
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
