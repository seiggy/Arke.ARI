using Arke.ARI.Actions;

namespace Arke.ARI
{
    public interface IAriActionClient
    {
        public IAsteriskActions Asterisk { get; }
        public IApplicationsActions Applications { get; }
        public IBridgesActions Bridges { get; }
        public IChannelsActions Channels { get; }
        public IDeviceStatesActions DeviceStates { get; }
        public IEndpointsActions Endpoints { get; }
        public IEventsActions Events { get; }
        public IMailboxesActions Mailboxes { get; }
        public IPlaybacksActions Playbacks { get; }
        public IRecordingsActions Recordings { get; }
        public ISoundsActions Sounds { get; }
    }
}
