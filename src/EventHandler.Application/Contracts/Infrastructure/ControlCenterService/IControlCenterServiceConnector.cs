using EventHandler.Domain.Models.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.ControlCenterService
{
    public interface IControlCenterServiceConnector
    {
        bool IsConnected { get; }
        Task InitializeAsync(ControlCenterServiceSettings ccsSettings, CancellationToken cancellationToken);
        void Stop();
    }
}
