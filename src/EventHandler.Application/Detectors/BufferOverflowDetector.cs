namespace EventHandler.Application.Detectors
{
    public static class BufferOverflowDetector
    {
        public static bool Detect(string buffer, int bufferLength)
            => buffer.Length >= bufferLength;
    }
}
