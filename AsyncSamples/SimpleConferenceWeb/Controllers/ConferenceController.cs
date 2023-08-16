using Arke.ARI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleConferenceWeb.Model;
using SimpleConferenceWeb.Services;
using System.Net;

namespace SimpleConferenceWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConferenceController : ControllerBase
    {
        private readonly ILogger<ConferenceController> _logger;
        private readonly AsteriskConferenceService _service;
        private readonly IServiceProvider _serviceProvider;

        public ConferenceController(ILogger<ConferenceController> logger, AsteriskConferenceService asteriskConferenceService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _service = asteriskConferenceService;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public IEnumerable<Conference> Get()
        {
            return _service.Conferences;
        }

        [HttpGet("{id}")]
        public Conference? Get([FromQuery] Guid id)
        {
            return _service.Conferences.SingleOrDefault(x => x.Id == id);
        }

        [HttpGet("{id}/Mute")]
        public async Task<HttpResponseMessage> Mute(Guid id)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);

            if (conf == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);

            await conf.MuteConference();

            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [HttpGet("{id}/Unmute")]
        public async Task<HttpResponseMessage> UnMute(Guid id)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);

            if (conf == null)
                return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            await conf.UnMuteConference();
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [HttpGet("{id}/Kick")]
        public async Task<HttpResponseMessage> Kick(Guid Id,[FromQuery] string channelId)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == Id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.RemoveUser(channelId);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("{id}/Play")]
        public async Task<HttpResponseMessage> Play(Guid id,[FromQuery] string audioFile)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.PlayFile(audioFile);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("{id}/StartRecording")]
        public async Task<HttpResponseMessage> StartRecord(Guid id, [FromQuery] string audioFile)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.StartRecording(audioFile);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("{id}/StopRecording")]
        public async Task<HttpResponseMessage> StopRecord(Guid id,  [FromQuery] string audioFile)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.StopRecording(audioFile);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("{id}/StartMOH")]
        public async Task<HttpResponseMessage> StartMOH(Guid id, [FromQuery] string mohClass)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.StartMOH(mohClass);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet("{id}/StopMOH")]
        public async Task<HttpResponseMessage> StopMOH(Guid id)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.StopMOH();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpPost]
        public async Task<Conference> Post(string name)
        {
            var conf = new Conference(
                Guid.NewGuid(), 
                name,
                _serviceProvider);
            await conf.StartConference();
            _service.Conferences.Add(conf);

            return conf;
        }

        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var conf = _service.Conferences.SingleOrDefault(x => x.Id == id);
            if (conf == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            await conf.DestroyConference();
            _service.Conferences.Remove(conf);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
