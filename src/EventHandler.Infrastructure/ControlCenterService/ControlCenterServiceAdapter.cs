using EventHandler.Application.Contracts.Infrastructure.ControlCenterService;
using EventHandler.Domain.Models.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ControlCenterService
{
    public class ControlCenterServiceAdapter : IControlCenterServiceAdapter
    {
        private const string DefaultHubName = "Service";
        private const int DefaultReconnectionDelayInSeconds = 12;

        private ControlCenterServiceInterface? _ccsInterface;
        public bool IsConnected { get; private set; }

        public event ActionEventHandler? OnReceiveAction;
        public event ErrorEventHandler? OnError;
        public event StateChangeEventHandler? OnStateChange;

        public string? Initialize(ControlCenterServiceSettings ccsSettings)
        {
            var ccsAddress = ccsSettings.ControlCenterServiceAddress ?? ccsSettings.ControlCenterServiceURL;
            var ccsHubName = ccsSettings.HubName ?? DefaultHubName;
            var ccsHubReconnectionDelay =
                ccsSettings.HubReconnectionDelay == 0
                ? DefaultReconnectionDelayInSeconds
                : ccsSettings.HubReconnectionDelay;

            if (ccsAddress != null)
            {
                _ccsInterface = new ControlCenterServiceInterface(
                        new List<string> { ccsAddress },
                        ccsHubName,
                        ccsSettings.HubReconnectionTries,
                        ccsHubReconnectionDelay,
                        ccsSettings.UseWebSocket);
            }

            if (_ccsInterface != null)
            {
                IsConnected = _ccsInterface.IsConnected;

                _ccsInterface.OnReceiveAction += OnReceiveAction;
                _ccsInterface.OnError += OnError;
                _ccsInterface.OnStateChange += OnStateChange;
            }

            return ccsAddress;
        }

        public async Task ConnectAsync(IAuthenticationObject auth)
        {
            if (_ccsInterface != null)
            {
                await _ccsInterface.ConnectAsync(auth);
            }
        }

        public ResultObject? Disconnect()
            => _ccsInterface?.Disconnect();

        public string? ConnectionStatus
            => _ccsInterface?.ConnectionStatus;
    }
}
