namespace Arke.ARI
{
    public class StasisEndpoint
    {
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }

        public StasisEndpoint(string host, int port, string username, string password)
        {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }
    }
}
