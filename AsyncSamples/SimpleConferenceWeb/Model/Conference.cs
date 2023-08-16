using Arke.ARI;
using Arke.ARI.Models;
using Microsoft.Extensions.Options;
using SimpleConferenceWeb.Services;
using System.Runtime.CompilerServices;

namespace SimpleConferenceWeb.Model
{
    public enum ConferenceState
    {
        Destroyed = -1, // Conference no longer valid
        Destroying = 0, // conf not ready, destroying bridge
        Creating = 1, // conf not ready, creating bridge
        Ready = 2, // Conf bridge is ready to accept channels
        ReadyWaiting = 3, // Conf is ready but playing MOH until members join, or admin joins (todo)
        Muted = 4, // no one can speak (check if MOH should be played)
        AdminMuted = 5 // only the admins can speak
    }

    public class Conference
    {
        private readonly ILogger<Conference> _logger;
        private readonly AriClient _ariClient;
        private ConferenceState _state;
        private readonly ARIConfig _config;
        private readonly IServiceProvider _serviceProvider;

        public Bridge? ConferenceBridge { get; set; }
        public string ConferenceName { get; set; }

        public List<ConferenceUser> ConferenceUsers { get; set; }
        public Guid Id { get; set; }

        public ConferenceState State
        {
            get { return _state; }
            set
            {
                _state = value;
                _logger.LogDebug("Conference {0} is now in state {1}", ConferenceName, State);
            }
        }

        public Conference(Guid id, string name, IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<Conference>>();
            _ariClient = serviceProvider.GetRequiredService<AriClient>();
            _config = serviceProvider.GetRequiredService<IOptions<ARIConfig>>().Value;
            ConferenceUsers = new List<ConferenceUser>();
            _serviceProvider = serviceProvider;
            Id = id;
            ConferenceName = name;
            State = ConferenceState.Creating;

            _ariClient.OnChannelDtmfReceivedEvent += _ariClient_OnChannelDtmfReceivedEvent;
            _ariClient.OnBridgeCreatedEvent += _ariClient_OnBridgeCreatedEvent;
            _ariClient.OnChannelEnteredBridgeEvent += _ariClient_OnChannelEnteredBridgeEvent;
            _ariClient.OnBridgeDestroyedEvent += _ariClient_OnBridgeDestroyedEvent;
            _ariClient.OnChannelLeftBridgeEvent += _ariClient_OnChannelLeftBridgeEvent;
            _ariClient.OnRecordingFinishedEvent += _ariClient_OnRecordingFinishedEvent;

            _ariClient.OnChannelTalkingStartedEvent += _ariClient_OnChannelTalkingStartedEvent;
            _ariClient.OnChannelTalkingFinishedEvent += _ariClient_OnChannelTalkingFinishedEvent;

            _logger.LogDebug($"Conference Init: {ConferenceName}");
        }

        private Task _ariClient_OnChannelTalkingFinishedEvent(IAriClient sender, ChannelTalkingFinishedEvent e)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.Channel.Id == e.Channel.Id);
            _logger.LogDebug($"User {confUser} finished talking in conference {ConferenceName} after {e.Duration} ms.");
            return Task.CompletedTask;
        }

        private Task _ariClient_OnChannelTalkingStartedEvent(IAriClient sender, ChannelTalkingStartedEvent e)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.Channel.Id == e.Channel.Id);
            _logger.LogDebug($"User {confUser} start talking in conference {ConferenceName}.");
            return Task.CompletedTask;
        }

        private async Task _ariClient_OnRecordingFinishedEvent(IAriClient sender, RecordingFinishedEvent e)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.CurrentRecodingId == e.Recording.Name);
            if (confUser == null) return;
            if (confUser.State != ConferenceUserState.RecordingName) return;

            confUser.State = ConferenceUserState.JoinConf;

            await _ariClient.Bridges.AddChannelAsync(ConferenceBridge.Id, confUser.Channel.Id, "ConfUser");
        }

        private async Task _ariClient_OnChannelLeftBridgeEvent(IAriClient sender, ChannelLeftBridgeEvent e)
        {
            await _ariClient.Bridges.PlayAsync(ConferenceBridge.Id, $"recording:conftemp-{e.Channel.Id}", "en", 0, 0, Guid.NewGuid().ToString());
            await _ariClient.Bridges.PlayAsync(ConferenceBridge.Id, "sound:conf-hasleft", "en", 0, 0, Guid.NewGuid().ToString());

            if (ConferenceUsers.Count() <= 1)
                await _ariClient.Bridges.StartMohAsync(ConferenceBridge.Id, "default");

            if (!ConferenceUsers.Any()) 
                await DestroyConference();

            await _ariClient.Recordings.DeleteStoredAsync($"conftemp-{e.Channel.Id}");
        }

        private Task _ariClient_OnBridgeDestroyedEvent(IAriClient sender, BridgeDestroyedEvent e)
        {
            if (e.Bridge.Id == ConferenceBridge.Id)
            {
                State = ConferenceState.Destroyed;
            }
            return Task.CompletedTask;
        }

        private async Task _ariClient_OnChannelEnteredBridgeEvent(IAriClient sender, ChannelEnteredBridgeEvent e)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.Channel.Id == e.Channel.Id);
            if (confUser == null) return;

            confUser.State = ConferenceUserState.InConf;

            if (ConferenceUsers.Count(x => x.State == ConferenceUserState.InConf) > 1) // are we the only ones here
            {
                // stop moh
                await _ariClient.Bridges.StopMohAsync(ConferenceBridge.Id);

                // change state
                State = ConferenceState.Ready;

                // announce new user
                await _ariClient.Bridges.PlayAsync(ConferenceBridge.Id, $"recording:{confUser.CurrentRecodingId}", "en", 0, 0, Guid.NewGuid().ToString());
                await _ariClient.Bridges.PlayAsync(ConferenceBridge.Id, "sound:conf-hasjoin", "en", 0, 0, Guid.NewGuid().ToString());
            }
            else
            {
                // only caller in conf
                await _ariClient.Channels.PlayAsync(e.Channel.Id, "sound:conf-onlyperson", "en", 0, 0, Guid.NewGuid().ToString());
            }
        }

        private Task _ariClient_OnBridgeCreatedEvent(IAriClient sender, BridgeCreatedEvent e)
        {
            if (e.Bridge.Id != Id.ToString()) return Task.CompletedTask;
            ConferenceBridge = e.Bridge;
            State = ConferenceState.Ready;
            _logger.LogDebug("Created Bridge {0} for {1}", Id, ConferenceName);
            return Task.CompletedTask;
        }

        private async Task _ariClient_OnChannelDtmfReceivedEvent(IAriClient sender, ChannelDtmfReceivedEvent e)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.Channel.Id == e.Channel.Id);
            if (confUser == null) return;

            // Pass digit to conference user
            await confUser.KeyPress(e.Digit);
        }

        public async Task<bool> StartConference()
        {
            _logger.LogDebug("Requesting new bridge {0} for {1}", Id, ConferenceName);
            var bridge = await _ariClient.Bridges.CreateAsync("mixing", Id.ToString(), ConferenceName);

            if (bridge == null) return false;

            _logger.LogDebug("Subscribing to events on bridge {0} for {1}", Id, ConferenceName);
            await _ariClient.Applications.SubscribeAsync(_config.AppName, $"bridge:{bridge.Id}");

            // start MOH
            await _ariClient.Bridges.StartMohAsync(bridge.Id, "default");

            State = ConferenceState.ReadyWaiting;
            ConferenceBridge = bridge;

            State = ConferenceState.Ready;

            return true;
        }

        public async Task<bool> AddUser(Channel c)
        {
            if (State == ConferenceState.Destroying) return false;
            if (State == ConferenceState.Destroyed)
            {
                if (!await StartConference()) 
                    return false;
            }

            if (State < ConferenceState.Ready) return false;

            await _ariClient.Channels.AnswerAsync(c.Id);
            await _ariClient.Channels.SetChannelVarAsync(c.Id, "TALK_DETECT(set)", "");

            ConferenceUsers.Add(new ConferenceUser(_serviceProvider.GetRequiredService<ILogger<ConferenceUser>>(), c, _ariClient, ConferenceUserType.Normal));

            return true;
        }

        public async Task RemoveUser(string channelId)
        {
            var confUser = ConferenceUsers.SingleOrDefault(x => x.Channel.Id == channelId);
            if (confUser == null) return;
            try
            {
                await _ariClient.Bridges.RemoveChannelAsync(ConferenceBridge.Id, channelId);
            }
            catch (AriException e)
            {
                if (e.StatusCode == 400)
                    _logger.LogDebug("Channel already disconnected from bridge");
            }
            ConferenceUsers.Remove(confUser);
        }

        public async Task PlayFile(string fileName)
        {
            await _ariClient.Bridges.PlayAsync(ConferenceBridge.Id, $"sound:{fileName}", "en", 0, 0, Guid.NewGuid().ToString());
        }

        public async Task StartRecording(string fileName)
        {
            await _ariClient.Bridges.RecordAsync(ConferenceBridge.Id, fileName, "wav", 0, 0, "fail", false, "none");
        }

        public async Task StopRecording(string fileName)
        {
            await _ariClient.Recordings.StopAsync(fileName);
        }

        public async Task MuteConference()
        {
            foreach (var conferenceUser in ConferenceUsers.Where(c => c.Type == ConferenceUserType.Normal)) 
            {
                await _ariClient.Channels.MuteAsync(conferenceUser.Channel.Id, "in");
            }
        }

        public async Task UnMuteConference()
        {
            foreach (var user in ConferenceUsers.Where(c => c.Type == ConferenceUserType.Normal))
            {
                await _ariClient.Channels.UnmuteAsync(user.Channel.Id, "in");
            }
        }

        public async Task StartMOH(string mohClass)
        {
            await _ariClient.Bridges.StartMohAsync(ConferenceBridge.Id, mohClass);
        }

        public async Task StopMOH()
        {
            await _ariClient.Bridges.StopMohAsync(ConferenceBridge.Id);
        }

        public async Task DestroyConference()
        {
            State = ConferenceState.Destroying;

            ConferenceUsers.ForEach(async x => await RemoveUser(x.Channel.Id));
            if (ConferenceBridge != null)
                await _ariClient.Bridges.DestroyAsync(ConferenceBridge.Id);
            ConferenceBridge = null;

            State = ConferenceState.Destroyed;
        }
    }
}
