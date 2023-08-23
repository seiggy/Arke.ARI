using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arke.ARI.Middleware
{
    public delegate Task AudioReceivedHandler(IExternalMediaProvider sender, byte[] audio);
    public delegate Task EndpointConnectedHandler(IExternalMediaProvider sender);

    public interface IExternalMediaProvider
    {
        ConnectionState State { get; }
        event AudioReceivedHandler OnAudioReceivedHandler;
        event EndpointConnectedHandler OnEndpointConnectedHandler;
    }
}
