namespace Arke.ARI.Middleware.ExternalMedia
{
    public interface IRTPAudioProcessor
    {
        Task<byte[]> GetAudioForPayload(byte[] rtpPayload, int rtpMarker);
    }
}