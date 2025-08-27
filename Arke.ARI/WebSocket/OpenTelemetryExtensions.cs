using OpenTelemetry.Trace;

namespace Arke.ARI.WebSocket
{
    public static class OpenTelemetryExtensions
    {
        public static TracerProviderBuilder AddAriWebSocketTracing(this TracerProviderBuilder builder)
        {
            return builder.AddSource(WebSocketEventProducer.ActivitySourceName);
        }
    }
}
