using EventHandler.Application.Contracts.Infrastructure.Persistence;
using System.Configuration;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.Persistence
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly ConfigurationDatabaseAccess _configurationDatabaseAccess;

        public ConfigurationRepository()
        {
            var dbConnectionString = ConfigurationManager.ConnectionStrings["EZ360Controllers"].ConnectionString;
            _configurationDatabaseAccess = new ConfigurationDatabaseAccess(dbConnectionString);
        }

        public async Task<ServiceConfiguration> GetConfigurationFromDbAsync()
            => await Task.Run(() => _configurationDatabaseAccess.GetServiceConfiguration(EControllerService.EventHandler));
    }
}
