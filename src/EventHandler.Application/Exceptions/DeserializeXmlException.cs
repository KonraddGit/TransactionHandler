using System;

namespace EventHandler.Application.Exceptions
{
    [Serializable]
    public class DeserializeXmlException : Exception
    {
        private const string DefaultMessage = "Deserializing XML file failed";

        public DeserializeXmlException() : base(DefaultMessage) { }

        public DeserializeXmlException(DeserializeXmlException exception, string message)
            : base($"{DefaultMessage}, {exception}, {message}") { }
    }
}
