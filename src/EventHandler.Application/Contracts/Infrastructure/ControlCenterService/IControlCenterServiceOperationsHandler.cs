using EventHandler.Domain.Models.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.ControlCenterService
{
    public interface IControlCenterServiceOperationsHandler
    {
        Task StartCCSConnectionAsync(ControlCenterServiceSettings ccsSettings, CancellationToken cancellationToken);
        void StopCCSConnection();
    }
}
