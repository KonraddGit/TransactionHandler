using System;

namespace EventHandler.Application.Exceptions
{
    [Serializable]
    public class ConvertEofException : Exception
    {
        private const string DefaultMessage = "Failed to convert eof characters";

        public ConvertEofException() : base(DefaultMessage) { }
    }
}
