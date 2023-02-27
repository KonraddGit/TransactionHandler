using System.Xml.Serialization;

namespace EventHandler.Domain.Models.Configuration.Enums
{
    public enum ClientRecognitionType
    {
        [XmlEnum]
        Full = 0,
        [XmlEnum]
        IPAddress = 1
    }
}