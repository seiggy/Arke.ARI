using Arke.ARI;
using Microsoft.Extensions.Options;
using SimpleConferenceWeb.Model;
using System.ComponentModel;

namespace SimpleConferenceWeb.Services
{
    public class AsteriskConferenceService : BackgroundService
    {
        private readonly ILogger<AsteriskConferenceService> _logger;
        private readonly AriClient _ariClient;
        private List<Conference> _conferences;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ARIConfig _config;
        private bool _shutdownRequested = false;

        public List<Conference> Conferences { get { return _conferences; } }

        public AsteriskConferenceService(ILogger<AsteriskConferenceService> logger, AriClient ariClient, IServiceProvider serviceProvider, IHostApplicationLifetime lifetime, IOptions<ARIConfig> options)
        {
            _logger = logger;
            _ariClient = ariClient;
            _ariClient.OnStasisEndEvent += _ariClient_OnStasisEndEvent;
            _ariClient.OnStasisStartEvent += _ariClient_OnStasisStartEvent;
            _serviceProvider = serviceProvider;
            _conferences= new List<Conference>();
            _lifetime = lifetime;
            _config = options.Value;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new Conference(Guid.NewGuid(), "test", _serviceProvider);
            await conf.StartConference();
            _conferences.Add(conf);
            _ariClient.Connect();
            await base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _conferences.ForEach(async x => await x.DestroyConference());
            _shutdownRequested = true;
            return base.StopAsync(cancellationToken);
        }

        private async Task _ariClient_OnStasisStartEvent(IAriClient sender, Arke.ARI.Models.StasisStartEvent e)
        {
            if (e.Application != _config.AppName) return;
            var failed = true;
            if (e.Args.Count == 0)
            {
                var addToConf = _conferences.FirstOrDefault();
                if (addToConf != null)
                {
                    await addToConf.AddUser(e.Channel);
                }
                return;
            }

            var confId = e.Args[0];
            var conf = _conferences.SingleOrDefault(x => x.ConferenceName == confId);

            if (conf == null)
            {
                await _ariClient.Channels.SetChannelVarAsync(e.Channel.Id, "CONFEXIT", "NOTFOUND");
            }
            else if (!await conf.AddUser(e.Channel))
            {
                await _ariClient.Channels.SetChannelVarAsync(e.Channel.Id, "CONFEXIT", "CANTJOIN");
            }
            else
            {
                _logger.LogDebug("Added channel {0} to conference {1}", e.Channel.Id, confId);
                failed = false;
            }

            if (failed)
            {
                await _ariClient.Channels.ContinueInDialplanAsync(e.Channel.Id,
                    e.Channel.Dialplan.Context,
                    e.Channel.Dialplan.Exten,
                    (int)e.Channel.Dialplan.Priority++);
            }
        }

        private async Task _ariClient_OnStasisEndEvent(IAriClient sender, Arke.ARI.Models.StasisEndEvent e)
        {
            if (e.Application != _config.AppName) return;

            var conf = _conferences.SingleOrDefault(x => x.ConferenceUsers.Any(c => c.Channel.Id == e.Channel.Id));
            if (conf == null) return;

            await conf.RemoveUser(e.Channel.Id);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                
                while(true)
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    if (_shutdownRequested)
                    {
                        throw new OperationCanceledException();
                    }
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (OperationCanceledException) 
            {
                _logger.LogWarning("Service shutting down...");
            }
        }
    }
}
