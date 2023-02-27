using EventHandler.Application.Contracts.Application;
using EventHandler.Application.Extensions;
using EventHandler.Domain.Models.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace EventHandler.Application.Configuration
{
    public class FileBackupManager : ConfigurationLocationProvider, IBackupManager
    {
        public async Task CreateBackupAsync(EventHandlerConfiguration? config)
        {
            var serializedConfig = XmlExtensions.Serialize(config);
            await File.WriteAllTextAsync(FileLocation, serializedConfig);
        }
    }
}
