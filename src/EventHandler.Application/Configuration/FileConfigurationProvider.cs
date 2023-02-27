using EventHandler.Application.Contracts.Application;
using EventHandler.Application.Extensions;
using EventHandler.Domain.Models.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace EventHandler.Application.Configuration
{
    public class FileConfigurationProvider : ConfigurationLocationProvider, IFileConfigurationProvider
    {
        public async Task<EventHandlerConfiguration?> ReadConfigurationAsync()
        {
            var backupConfig = await File.ReadAllTextAsync(FileLocation);

            return XmlExtensions.Deserialize<EventHandlerConfiguration>(backupConfig);
        }
    }
}
