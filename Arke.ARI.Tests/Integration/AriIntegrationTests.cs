using System;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Arke.ARI.Tests.Integration
{
    /// <summary>
    /// Integration tests that test against the actual Asterisk environment
    /// These tests verify that the Roslyn code generator is working properly
    /// </summary>
    public class AriIntegrationTests : TestBase, IAsyncLifetime
    {
        private AriClient _client;
        private bool _isConnected;

        public AriIntegrationTests(ITestOutputHelper output) : base(output)
        {
        }

        public async Task InitializeAsync()
        {
            _client = CreateTestClient("integration-test");
            Logger.LogInformation("Initializing integration tests");
        }

        public new async Task DisposeAsync()
        {
            if (_isConnected)
            {
                try
                {
                    await _client.Disconnect();
                    Logger.LogInformation("Disconnected from Asterisk");
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error disconnecting from Asterisk");
                }
            }

            if (_client != null)
                await _client.DisposeAsync();
            await base.DisposeAsync();
        }

        [Fact]
        public async Task Connect_ToAsteriskEnvironment_Succeeds()
        {
            // Act
            await AssertAsyncOperation(async () =>
            {
                await _client.Connect(true);
            }, "Connect to Asterisk");

            // Assert
            Assert.True(_client.Connected);
        }

        [Fact]
        public async Task AsteriskActions_GetInfo_ReturnsValidInfo()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var info = await AssertAsyncOperation(async () =>
                await _client.Asterisk.GetInfoAsync(), "Get Asterisk Info");

            // Assert
            Assert.NotNull(info);
            Assert.NotNull(info.Build);
            Assert.NotNull(info.System);
            Assert.NotNull(info.Config);
            Assert.NotNull(info.Status);
        }

        [Fact]
        public async Task ApplicationsActions_List_ReturnsValidApplications()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var applications = await AssertAsyncOperation(async () =>
                await _client.Applications.ListAsync(), "List Applications");

            // Assert
            Assert.NotNull(applications);
            Assert.NotEmpty(applications);

            // Verify our test application is in the list
            var testApp = applications.Find(app => app.Name == "integration-test");
            Assert.NotNull(testApp);
        }

        [Fact]
        public async Task ApplicationsActions_Get_ReturnsValidApplication()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var application = await AssertAsyncOperation(async () =>
                await _client.Applications.GetAsync("integration-test"), "Get Application");

            // Assert
            Assert.NotNull(application);
            Assert.Equal("integration-test", application.Name);
        }

        [Fact]
        public async Task BridgesActions_CreateAndDestroy_Succeeds()
        {
            // Arrange
            await ConnectToAsterisk();
            var bridgeId = Guid.NewGuid().ToString();

            // Act & Assert
            var bridge = await AssertAsyncOperation(async () =>
                await _client.Bridges.CreateAsync("mixing", bridgeId, "integration-test"), "Create Bridge");

            Assert.NotNull(bridge);
            Assert.Equal(bridgeId, bridge.Id);

            // Cleanup
            await AssertAsyncOperation(async () =>
                await _client.Bridges.DestroyAsync(bridgeId), "Destroy Bridge");
        }

        [Fact]
        public async Task BridgesActions_List_ReturnsValidBridges()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var bridges = await AssertAsyncOperation(async () =>
                await _client.Bridges.ListAsync(), "List Bridges");

            // Assert
            Assert.NotNull(bridges);
        }

        [Fact]
        public async Task ChannelsActions_List_ReturnsValidChannels()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var channels = await AssertAsyncOperation(async () =>
                await _client.Channels.ListAsync(), "List Channels");

            // Assert
            Assert.NotNull(channels);
        }

        [Fact]
        public async Task DeviceStatesActions_List_ReturnsValidDeviceStates()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var deviceStates = await AssertAsyncOperation(async () =>
                await _client.DeviceStates.ListAsync(), "List Device States");

            // Assert
            Assert.NotNull(deviceStates);
        }

        [Fact]
        public async Task EndpointsActions_List_ReturnsValidEndpoints()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var endpoints = await AssertAsyncOperation(async () =>
                await _client.Endpoints.ListAsync(), "List Endpoints");

            // Assert
            Assert.NotNull(endpoints);
        }

        [Fact]
        public async Task MailboxesActions_List_ReturnsValidMailboxes()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var mailboxes = await AssertAsyncOperation(async () =>
                await _client.Mailboxes.ListAsync(), "List Mailboxes");

            // Assert
            Assert.NotNull(mailboxes);
        }

        [Fact]
        public async Task RecordingsActions_ListStored_ReturnsValidRecordings()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var recordings = await AssertAsyncOperation(async () =>
                await _client.Recordings.ListStoredAsync(), "List Stored Recordings");

            // Assert
            Assert.NotNull(recordings);
        }

        [Fact]
        public async Task SoundsActions_List_ReturnsValidSounds()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act
            var sounds = await AssertAsyncOperation(async () =>
                await _client.Sounds.ListAsync(), "List Sounds");

            // Assert
            Assert.NotNull(sounds);
        }

        [Fact]
        public async Task CodeGenerator_AllActionsAreGenerated_AndAccessible()
        {
            // Arrange
            await ConnectToAsterisk();

            // Act & Assert - Test that all generated action classes are accessible
            Assert.NotNull(_client.Asterisk);
            Assert.NotNull(_client.Applications);
            Assert.NotNull(_client.Bridges);
            Assert.NotNull(_client.Channels);
            Assert.NotNull(_client.DeviceStates);
            Assert.NotNull(_client.Endpoints);
            Assert.NotNull(_client.Events);
            Assert.NotNull(_client.Mailboxes);
            Assert.NotNull(_client.Playbacks);
            Assert.NotNull(_client.Recordings);
            Assert.NotNull(_client.Sounds);

            // Test that methods are callable (even if they might fail due to no data)
            await AssertAsyncOperation(async () =>
                await _client.Asterisk.GetInfoAsync(), "Asterisk GetInfo");

            await AssertAsyncOperation(async () =>
                await _client.Applications.ListAsync(), "Applications List");

            await AssertAsyncOperation(async () =>
                await _client.Bridges.ListAsync(), "Bridges List");

            await AssertAsyncOperation(async () =>
                await _client.Channels.ListAsync(), "Channels List");

            await AssertAsyncOperation(async () =>
                await _client.DeviceStates.ListAsync(), "DeviceStates List");

            await AssertAsyncOperation(async () =>
                await _client.Endpoints.ListAsync(), "Endpoints List");

            await AssertAsyncOperation(async () =>
                await _client.Mailboxes.ListAsync(), "Mailboxes List");

            await AssertAsyncOperation(async () =>
                await _client.Recordings.ListStoredAsync(), "Recordings ListStored");

            await AssertAsyncOperation(async () =>
                await _client.Sounds.ListAsync(), "Sounds List");
        }

        private async Task ConnectToAsterisk()
        {
            if (!_isConnected)
            {
                await _client.Connect(false);
                _isConnected = true;
                Logger.LogInformation("Connected to Asterisk for integration test");
            }
        }
    }
}
