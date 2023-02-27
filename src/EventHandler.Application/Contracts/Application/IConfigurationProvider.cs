using EventHandler.Domain.Models.Configuration;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Application
{
    public interface IConfigurationProvider
    {
        Task<EventHandlerConfiguration?> ReadConfigurationAsync();
    }
}
