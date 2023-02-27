using System.IO;

namespace EventHandler.Application.Configuration
{
    public class ConfigurationLocationProvider
    {
        private const string ConfigurationXml = "Configuration.xml";

        public string? FileLocation
            => $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\{ConfigurationXml}";
    }
}
