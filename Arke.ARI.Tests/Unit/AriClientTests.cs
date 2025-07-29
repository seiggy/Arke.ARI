using System;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Arke.ARI.Tests.Unit
{
    public class AriClientTests : TestBase
    {
        public AriClientTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = CreateTestClient();

            // Assert
            Assert.NotNull(client);
            Assert.NotNull(client.Asterisk);
            Assert.NotNull(client.Applications);
            Assert.NotNull(client.Bridges);
            Assert.NotNull(client.Channels);
            Assert.NotNull(client.DeviceStates);
            Assert.NotNull(client.Endpoints);
            Assert.NotNull(client.Events);
            Assert.NotNull(client.Mailboxes);
            Assert.NotNull(client.Playbacks);
            Assert.NotNull(client.Recordings);
            Assert.NotNull(client.Sounds);
        }

        [Fact]
        public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AriClient(TestEndpoint, null, "test-app"));

            Assert.Equal("serviceProvider", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullEndpoint_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AriClient(null, ServiceProvider, "test-app"));

            Assert.Equal("endPoint", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithEmptyApplication_ThrowsArgumentException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() =>
                new AriClient(TestEndpoint, ServiceProvider, ""));

            Assert.Equal("application", ex.ParamName);
        }

        [Fact]
        public void Constructor_WithNullApplication_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
                new AriClient(TestEndpoint, ServiceProvider, null));

            Assert.Equal("application", ex.ParamName);
        }

        [Fact]
        public void Connected_WhenNotConnected_ReturnsFalse()
        {
            // Arrange
            var client = CreateTestClient();

            // Act
            var connected = client.Connected;

            // Assert
            Assert.False(connected);
        }

        [Fact]
        public void EventDispatchingStrategy_DefaultValue_IsThreadPool()
        {
            // Arrange
            var client = CreateTestClient();

            // Act
            var strategy = client.EventDispatchingStrategy;

            // Assert
            Assert.Equal(EventDispatchingStrategy.ThreadPool, strategy);
        }

        [Fact]
        public void EventDispatchingStrategy_CanBeSet()
        {
            // Arrange
            var client = CreateTestClient();

            // Act
            client.EventDispatchingStrategy = EventDispatchingStrategy.DedicatedThread;

            // Assert
            Assert.Equal(EventDispatchingStrategy.DedicatedThread, client.EventDispatchingStrategy);
        }

        [Fact]
        public void Dispose_WhenCalled_DisposesResources()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert
            Assert.Null(Record.Exception(() => client.Dispose()));
        }

        [Fact]
        public void Dispose_WhenCalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert
            client.Dispose();
            Assert.Null(Record.Exception(() => client.Dispose()));
        }

        [Fact]
        public async Task Connect_WithValidParameters_DoesNotThrow()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert
            await AssertAsyncOperation(async () => await client.Connect(false), "Connect");
        }

        [Fact]
        public async Task Disconnect_WhenNotConnected_DoesNotThrow()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert
            await AssertAsyncOperation(async () => await client.Disconnect(), "Disconnect");
        }

        [Theory]
        [InlineData("test-app")]
        [InlineData("my-application")]
        [InlineData("ari-test")]
        public void Constructor_WithDifferentApplicationNames_CreatesInstance(string applicationName)
        {
            // Act
            var client = CreateTestClient(applicationName);

            // Assert
            Assert.NotNull(client);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public void Constructor_WithDifferentOptions_CreatesInstance(bool subscribeAllEvents, bool ssl)
        {
            // Act
            var client = new AriClient(TestEndpoint, ServiceProvider, "test-app", subscribeAllEvents, ssl);

            // Assert
            Assert.NotNull(client);
        }
    }
}
