using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace EventHandler.Application.Extensions
{
    public static class XmlExtensions
    {
        public static string Serialize<T>(T value)
        {
            var serializer = new XmlSerializer(typeof(T));

            using var stringWriter = new StringWriter();
            using var xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };

            serializer.Serialize(xmlWriter, value);

            return stringWriter.ToString();
        }

        public static T Deserialize<T>(string source, XmlRootAttribute? xmlRoot = null)
        {
            var deserializer = xmlRoot != null
              ? new XmlSerializer(typeof(T), xmlRoot)
              : new XmlSerializer(typeof(T));

            using var reader = new StringReader(source);
            return (T)deserializer.Deserialize(reader);
        }
    }
}
