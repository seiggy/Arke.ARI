using Microsoft.CognitiveServices.Speech.Audio;

namespace AudioTranscriptionSample
{
    internal class BinaryAudioStreamReader : PullAudioInputStreamCallback
    {
        private readonly Stream _stream;
                
        public BinaryAudioStreamReader(Stream stream)
        {
            _stream = stream;
        }

        public async Task CopyToStream(byte[] buffer)
        {
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }

        public override int Read(byte[] dataBuffer, uint size)
        {
            return _stream.Read(dataBuffer, 0, (int)size);
        }

        public override void Close()
        {
            _stream.Close();
            _stream.Dispose();
            base.Close();
        }
    }
}