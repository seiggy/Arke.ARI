namespace Arke.ARI.Middleware.ExternalMedia
{
    public class AudioFormat
    {
        public AudioFormat() { }
        public Codec Codec { get; set; }
        public int SamplesPerSecond { get; set; }
        public int BitRate { get; set; }
        public int Channels { get; set; }
    }

    public enum Codec
    {
        PCM
    }
}