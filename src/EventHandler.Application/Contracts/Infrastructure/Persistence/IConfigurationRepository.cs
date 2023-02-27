using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.Persistence
{
    public interface IConfigurationRepository
    {
        Task<ServiceConfiguration> GetConfigurationFromDbAsync();
    }
}
