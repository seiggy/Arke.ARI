/*
   Arke ARI Framework
   Automatically generated file @ 6/23/2023 11:34:36 AM
*/
using System.Collections.Generic;
using System.Linq;
using Arke.ARI.Middleware;
using Arke.ARI.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Arke.ARI.Actions
{

    public class MailboxesActions : ARIBaseAction, IMailboxesActions
    {

        public MailboxesActions(IActionConsumer consumer)
            : base(consumer)
        { }

        /// <summary>
        /// List all mailboxes.. 
        /// </summary>
        public virtual List<Mailbox> List()
        {
            string path = "mailboxes";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = Execute<List<Mailbox>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Retrieve the current state of a mailbox.. 
        /// </summary>
        /// <param name="mailboxName">Name of the mailbox</param>
        public virtual Mailbox Get(string mailboxName)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);

            var response = Execute<Mailbox>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Change the state of a mailbox. (Note - implicitly creates the mailbox).. 
        /// </summary>
        /// <param name="mailboxName">Name of the mailbox</param>
        /// <param name="oldMessages">Count of old messages in the mailbox</param>
        /// <param name="newMessages">Count of new messages in the mailbox</param>
        public virtual void Update(string mailboxName, int oldMessages, int newMessages)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);
            if (oldMessages != null)
                request.AddParameter("oldMessages", oldMessages, ParameterType.QueryString);
            if (newMessages != null)
                request.AddParameter("newMessages", newMessages, ParameterType.QueryString);
            var response = Execute(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Destroy a mailbox.. 
        /// </summary>
        /// <param name="mailboxName">Name of the mailbox</param>
        public virtual void Delete(string mailboxName)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);
            var response = Execute(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }

        /// <summary>
        /// List all mailboxes.. 
        /// </summary>
        public virtual async Task<List<Mailbox>> ListAsync()
        {
            string path = "mailboxes";
            var request = GetNewRequest(path, HttpMethod.GET);

            var response = await ExecuteTask<List<Mailbox>>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Retrieve the current state of a mailbox.. 
        /// </summary>
        public virtual async Task<Mailbox> GetAsync(string mailboxName)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.GET);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);

            var response = await ExecuteTask<Mailbox>(request);

            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return response.Data;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Change the state of a mailbox. (Note - implicitly creates the mailbox).. 
        /// </summary>
        public virtual async Task UpdateAsync(string mailboxName, int oldMessages, int newMessages)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.PUT);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);
            if (oldMessages != null)
                request.AddParameter("oldMessages", oldMessages, ParameterType.QueryString);
            if (newMessages != null)
                request.AddParameter("newMessages", newMessages, ParameterType.QueryString);
            var response = await ExecuteTask(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
        /// <summary>
        /// Destroy a mailbox.. 
        /// </summary>
        public virtual async Task DeleteAsync(string mailboxName)
        {
            string path = "mailboxes/{mailboxName}";
            var request = GetNewRequest(path, HttpMethod.DELETE);
            if (mailboxName != null)
                request.AddUrlSegment("mailboxName", mailboxName);
            var response = await ExecuteTask(request);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                return;
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new AriException("Mailbox not found", (int)response.StatusCode);
                default:
                    // Unknown server response
                    throw new AriException(string.Format("Unknown response code {0} from ARI.", response.StatusCode), (int)response.StatusCode);
            }
        }
    }
}

