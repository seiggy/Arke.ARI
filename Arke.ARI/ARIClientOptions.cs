namespace Arke.ARI
{
    public class ARIClientOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:8088/ari";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string HttpClientName { get; set; } = "ARIClient";
        public string ApplicationName { get; set; } = null;
        public bool AutoReconnect { get; set; } = true;
        public int ReconnectDelay { get; set; } = 5000;
        public int MaxReconnectAttempts { get; set; } = 10;
        public int ConnectionTimeout { get; set; } = 10000;
        public int ReceiveBufferSize { get; set; } = 8192;
        public bool SubscribeAllEvents { get; set; } = true;
        public bool UseSecureWebSocket { get; set; } = false;
    }
}
