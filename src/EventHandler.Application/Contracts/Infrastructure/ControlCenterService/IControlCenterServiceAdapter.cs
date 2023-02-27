using EventHandler.Domain.Models.Configuration;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.ControlCenterService
{
    public interface IControlCenterServiceAdapter
    {
        bool IsConnected { get; }
        string? ConnectionStatus { get; }
        string? Initialize(ControlCenterServiceSettings ccsSettings);
        Task ConnectAsync(IAuthenticationObject auth);
        ResultObject? Disconnect();

        event ActionEventHandler OnReceiveAction;
        event ErrorEventHandler OnError;
        event StateChangeEventHandler? OnStateChange;
    }
}
