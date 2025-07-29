using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Arke.ARI;
using Arke.ARI.Actions;
using Arke.ARI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Arke.ARI.Tests.CodeGenerator
{
    /// <summary>
    /// Tests specifically for the Roslyn code generator functionality
    /// These tests verify that the code generator is working correctly
    /// </summary>
    public class CodeGeneratorTests : TestBase
    {
        public CodeGeneratorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CodeGenerator_AllActionInterfacesAreGenerated()
        {
            // Act & Assert - Verify all action interfaces exist
            Assert.NotNull(typeof(IAsteriskActions));
            Assert.NotNull(typeof(IApplicationsActions));
            Assert.NotNull(typeof(IBridgesActions));
            Assert.NotNull(typeof(IChannelsActions));
            Assert.NotNull(typeof(IDeviceStatesActions));
            Assert.NotNull(typeof(IEndpointsActions));
            Assert.NotNull(typeof(IEventsActions));
            Assert.NotNull(typeof(IMailboxesActions));
            Assert.NotNull(typeof(IPlaybacksActions));
            Assert.NotNull(typeof(IRecordingsActions));
            Assert.NotNull(typeof(ISoundsActions));
        }

        [Fact]
        public void CodeGenerator_AllActionClassesAreGenerated()
        {
            // Act & Assert - Verify all action classes exist
            Assert.NotNull(typeof(AsteriskActions));
            Assert.NotNull(typeof(ApplicationsActions));
            Assert.NotNull(typeof(BridgesActions));
            Assert.NotNull(typeof(ChannelsActions));
            Assert.NotNull(typeof(DeviceStatesActions));
            Assert.NotNull(typeof(EndpointsActions));
            Assert.NotNull(typeof(EventsActions));
            Assert.NotNull(typeof(MailboxesActions));
            Assert.NotNull(typeof(PlaybacksActions));
            Assert.NotNull(typeof(RecordingsActions));
            Assert.NotNull(typeof(SoundsActions));
        }

        [Fact]
        public void CodeGenerator_ActionClassesImplementInterfaces()
        {
            // Act & Assert - Verify all action classes implement their interfaces
            Assert.True(typeof(AsteriskActions).GetInterfaces().Contains(typeof(IAsteriskActions)));
            Assert.True(typeof(ApplicationsActions).GetInterfaces().Contains(typeof(IApplicationsActions)));
            Assert.True(typeof(BridgesActions).GetInterfaces().Contains(typeof(IBridgesActions)));
            Assert.True(typeof(ChannelsActions).GetInterfaces().Contains(typeof(IChannelsActions)));
            Assert.True(typeof(DeviceStatesActions).GetInterfaces().Contains(typeof(IDeviceStatesActions)));
            Assert.True(typeof(EndpointsActions).GetInterfaces().Contains(typeof(IEndpointsActions)));
            Assert.True(typeof(EventsActions).GetInterfaces().Contains(typeof(IEventsActions)));
            Assert.True(typeof(MailboxesActions).GetInterfaces().Contains(typeof(IMailboxesActions)));
            Assert.True(typeof(PlaybacksActions).GetInterfaces().Contains(typeof(IPlaybacksActions)));
            Assert.True(typeof(RecordingsActions).GetInterfaces().Contains(typeof(IRecordingsActions)));
            Assert.True(typeof(SoundsActions).GetInterfaces().Contains(typeof(ISoundsActions)));
        }

        [Fact]
        public void CodeGenerator_AsteriskActionsMethodsAreGenerated()
        {
            // Arrange
            var asteriskActionsType = typeof(IAsteriskActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(asteriskActionsType.GetMethod("GetInfoAsync"));
            Assert.NotNull(asteriskActionsType.GetMethod("GetBuildInfoAsync"));
            Assert.NotNull(asteriskActionsType.GetMethod("GetConfigInfoAsync"));
            Assert.NotNull(asteriskActionsType.GetMethod("GetSystemInfoAsync"));
            Assert.NotNull(asteriskActionsType.GetMethod("PingAsync"));
        }

        [Fact]
        public void CodeGenerator_ApplicationsActionsMethodsAreGenerated()
        {
            // Arrange
            var applicationsActionsType = typeof(IApplicationsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(applicationsActionsType.GetMethod("ListAsync"));
            Assert.NotNull(applicationsActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(applicationsActionsType.GetMethod("SubscribeAsync", new[] { typeof(string), typeof(string) }));
            Assert.NotNull(applicationsActionsType.GetMethod("UnsubscribeAsync", new[] { typeof(string), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_BridgesActionsMethodsAreGenerated()
        {
            // Arrange
            var bridgesActionsType = typeof(IBridgesActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(bridgesActionsType.GetMethod("ListAsync"));
            Assert.NotNull(bridgesActionsType.GetMethod("CreateAsync", new[] { typeof(string), typeof(string), typeof(string) }));
            Assert.NotNull(bridgesActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(bridgesActionsType.GetMethod("DestroyAsync", new[] { typeof(string) }));
            Assert.NotNull(bridgesActionsType.GetMethod("AddChannelAsync", new[] { typeof(string), typeof(string), typeof(string) }));
            Assert.NotNull(bridgesActionsType.GetMethod("RemoveChannelAsync", new[] { typeof(string), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_ChannelsActionsMethodsAreGenerated()
        {
            // Arrange
            var channelsActionsType = typeof(IChannelsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(channelsActionsType.GetMethod("ListAsync"));
            Assert.NotNull(channelsActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(channelsActionsType.GetMethod("AnswerAsync", new[] { typeof(string) }));
            Assert.NotNull(channelsActionsType.GetMethod("HangupAsync", new[] { typeof(string), typeof(string) }));
            Assert.NotNull(channelsActionsType.GetMethod("PlayAsync", new[] { typeof(string), typeof(string) }));
            Assert.NotNull(channelsActionsType.GetMethod("RecordAsync", new[] { typeof(string), typeof(string), typeof(string), typeof(int), typeof(int), typeof(string), typeof(bool), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_DeviceStatesActionsMethodsAreGenerated()
        {
            // Arrange
            var deviceStatesActionsType = typeof(IDeviceStatesActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(deviceStatesActionsType.GetMethod("ListAsync"));
            Assert.NotNull(deviceStatesActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(deviceStatesActionsType.GetMethod("UpdateAsync", new[] { typeof(string), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_EndpointsActionsMethodsAreGenerated()
        {
            // Arrange
            var endpointsActionsType = typeof(IEndpointsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(endpointsActionsType.GetMethod("ListAsync"));
            Assert.NotNull(endpointsActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(endpointsActionsType.GetMethod("SendMessageAsync", new[] { typeof(string), typeof(string), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_EventsActionsMethodsAreGenerated()
        {
            // Arrange
            var eventsActionsType = typeof(IEventsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(eventsActionsType.GetMethod("ListAsync"));
        }

        [Fact]
        public void CodeGenerator_MailboxesActionsMethodsAreGenerated()
        {
            // Arrange
            var mailboxesActionsType = typeof(IMailboxesActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(mailboxesActionsType.GetMethod("ListAsync"));
            Assert.NotNull(mailboxesActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(mailboxesActionsType.GetMethod("UpdateAsync", new[] { typeof(string), typeof(int), typeof(int) }));
            Assert.NotNull(mailboxesActionsType.GetMethod("DeleteAsync", new[] { typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_PlaybacksActionsMethodsAreGenerated()
        {
            // Arrange
            var playbacksActionsType = typeof(IPlaybacksActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(playbacksActionsType.GetMethod("ListAsync"));
            Assert.NotNull(playbacksActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
            Assert.NotNull(playbacksActionsType.GetMethod("StopAsync", new[] { typeof(string) }));
            Assert.NotNull(playbacksActionsType.GetMethod("ControlAsync", new[] { typeof(string), typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_RecordingsActionsMethodsAreGenerated()
        {
            // Arrange
            var recordingsActionsType = typeof(IRecordingsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(recordingsActionsType.GetMethod("ListStoredAsync"));
            Assert.NotNull(recordingsActionsType.GetMethod("GetStoredAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("CopyStoredAsync", new[] { typeof(string), typeof(string), typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("DeleteStoredAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("GetLiveAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("CancelAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("StopAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("MuteAsync", new[] { typeof(string) }));
            Assert.NotNull(recordingsActionsType.GetMethod("UnmuteAsync", new[] { typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_SoundsActionsMethodsAreGenerated()
        {
            // Arrange
            var soundsActionsType = typeof(ISoundsActions);

            // Act & Assert - Verify key methods exist
            Assert.NotNull(soundsActionsType.GetMethod("ListAsync"));
            Assert.NotNull(soundsActionsType.GetMethod("GetAsync", new[] { typeof(string) }));
        }

        [Fact]
        public void CodeGenerator_AllModelClassesAreGenerated()
        {
            // Act & Assert - Verify key model classes exist
            Assert.NotNull(typeof(Application));
            Assert.NotNull(typeof(AsteriskInfo));
            Assert.NotNull(typeof(Bridge));
            Assert.NotNull(typeof(Channel));
            Assert.NotNull(typeof(DeviceState));
            Assert.NotNull(typeof(Endpoint));
            Assert.NotNull(typeof(Event));
            Assert.NotNull(typeof(Mailbox));
            Assert.NotNull(typeof(Playback));
            Assert.NotNull(typeof(LiveRecording));
            Assert.NotNull(typeof(StoredRecording));
            Assert.NotNull(typeof(Sound));
        }

        [Fact]
        public void CodeGenerator_AllEventClassesAreGenerated()
        {
            // Act & Assert - Verify key event classes exist
            Assert.NotNull(typeof(StasisStartEvent));
            Assert.NotNull(typeof(StasisEndEvent));
            Assert.NotNull(typeof(ChannelCreatedEvent));
            Assert.NotNull(typeof(ChannelDestroyedEvent));
            Assert.NotNull(typeof(ChannelDtmfReceivedEvent));
            Assert.NotNull(typeof(BridgeCreatedEvent));
            Assert.NotNull(typeof(BridgeDestroyedEvent));
            Assert.NotNull(typeof(RecordingStartedEvent));
            Assert.NotNull(typeof(RecordingFinishedEvent));
            Assert.NotNull(typeof(PlaybackStartedEvent));
            Assert.NotNull(typeof(PlaybackFinishedEvent));
        }

        [Fact]
        public void CodeGenerator_MethodSignaturesAreCorrect()
        {
            // Arrange
            var asteriskActionsType = typeof(IAsteriskActions);
            var getInfoMethod = asteriskActionsType.GetMethod("GetInfoAsync");

            // Act & Assert - Verify method signature is correct
            Assert.NotNull(getInfoMethod);
            Assert.Equal(typeof(Task<AsteriskInfo>), getInfoMethod.ReturnType);
            Assert.Empty(getInfoMethod.GetParameters());
        }

        [Fact]
        public void CodeGenerator_AsyncMethodsReturnTasks()
        {
            // Arrange
            var actionTypes = new[]
            {
                typeof(IAsteriskActions),
                typeof(IApplicationsActions),
                typeof(IBridgesActions),
                typeof(IChannelsActions),
                typeof(IDeviceStatesActions),
                typeof(IEndpointsActions),
                typeof(IEventsActions),
                typeof(IMailboxesActions),
                typeof(IPlaybacksActions),
                typeof(IRecordingsActions),
                typeof(ISoundsActions)
            };

            // Act & Assert - Verify all methods return Task or Task<T>
            foreach (var actionType in actionTypes)
            {
                var methods = actionType.GetMethods();
                foreach (var method in methods)
                {
                    Assert.True(method.ReturnType == typeof(Task) ||
                               method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>),
                               $"Method {method.Name} in {actionType.Name} does not return Task or Task<T>");
                }
            }
        }

        [Fact]
        public void CodeGenerator_ClientPropertiesAreAccessible()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert - Verify all action properties are accessible
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
        public void CodeGenerator_ActionPropertiesAreCorrectTypes()
        {
            // Arrange
            var client = CreateTestClient();

            // Act & Assert - Verify action properties are correct types
            Assert.IsAssignableFrom<IAsteriskActions>(client.Asterisk);
            Assert.IsAssignableFrom<IApplicationsActions>(client.Applications);
            Assert.IsAssignableFrom<IBridgesActions>(client.Bridges);
            Assert.IsAssignableFrom<IChannelsActions>(client.Channels);
            Assert.IsAssignableFrom<IDeviceStatesActions>(client.DeviceStates);
            Assert.IsAssignableFrom<IEndpointsActions>(client.Endpoints);
            Assert.IsAssignableFrom<IEventsActions>(client.Events);
            Assert.IsAssignableFrom<IMailboxesActions>(client.Mailboxes);
            Assert.IsAssignableFrom<IPlaybacksActions>(client.Playbacks);
            Assert.IsAssignableFrom<IRecordingsActions>(client.Recordings);
            Assert.IsAssignableFrom<ISoundsActions>(client.Sounds);
        }
    }
}
