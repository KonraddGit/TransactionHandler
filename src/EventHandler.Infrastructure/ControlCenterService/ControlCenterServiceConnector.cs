using EventHandler.Application;
using EventHandler.Application.Contracts.Infrastructure.ControlCenterService;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ControlCenterService
{
    public class ControlCenterServiceConnector : IControlCenterServiceConnector
    {
        private const int ReconnectDelayInMilliseconds = 30_000;

        private readonly ILogger _logger;
        private readonly IControlCenterEventDispatcher _eventDispatcher;
        private readonly IControlCenterServiceAdapter _ccsInterfaceAdapter;
        private readonly IHttpClientFactory _httpClientFactory;

        public bool IsConnected => _ccsInterfaceAdapter.IsConnected;

        public ControlCenterServiceConnector(
            ILogger<ControlCenterServiceConnector> logger,
            IControlCenterEventDispatcher eventDispatcher,
            IControlCenterServiceAdapter ccsInterfaceAdapter,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _ccsInterfaceAdapter = ccsInterfaceAdapter;
            _httpClientFactory = httpClientFactory;
            _eventDispatcher = eventDispatcher;
        }

        public async Task InitializeAsync(ControlCenterServiceSettings? ccsSettings, CancellationToken cancellationToken)
        {
            if (ccsSettings == null)
            {
                return;
            }

            _logger.LogInformation("Initializing ControlCenterService connection");
            var ccsAddress = _ccsInterfaceAdapter.Initialize(ccsSettings);

            try
            {
                _ccsInterfaceAdapter.OnStateChange += _eventDispatcher.DispatchState;
                _ccsInterfaceAdapter.OnReceiveAction += _eventDispatcher.DispatchAction;
                _ccsInterfaceAdapter.OnError += _eventDispatcher.DispatchError;

                var httpClient = _httpClientFactory.CreateClient("ControlCenterService");
                var httpResponseMessage = await httpClient.GetAsync($"{ccsAddress}/signalr/hubs", cancellationToken);

                if (!_ccsInterfaceAdapter.IsConnected && httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("ControlCenterService available {CcsUrl}", ccsAddress);

                    var auth = new Auth(ccsSettings.ServiceName, AssemblyInformation.SoftwareVersion, ccsAddress)
                    {
                        ServiceType = ServiceType.CONNECTION
                    };

                    await _ccsInterfaceAdapter.ConnectAsync(auth);

                    _logger.LogInformation("Connection status: {Status}", _ccsInterfaceAdapter.ConnectionStatus);
                }
                else
                {
                    _logger.LogError("ControlCenterService is not available {CcsUrl}, {StatusCode}:{Content}",
                        ccsAddress, httpResponseMessage.StatusCode, httpResponseMessage.Content);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not establish connection with ControlCenterService");
                await ReconnectAsync(ccsSettings, cancellationToken);
            }
        }

        public void Stop()
        {
            try
            {
                _ccsInterfaceAdapter.OnStateChange -= _eventDispatcher.DispatchState;
                _ccsInterfaceAdapter.OnReceiveAction -= _eventDispatcher.DispatchAction;
                _ccsInterfaceAdapter.OnError -= _eventDispatcher.DispatchError;

                var result = _ccsInterfaceAdapter.Disconnect();

                if (result?.Exception != null)
                {
                    _logger.LogError(result.Exception, "ControlCenterService disconnection error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ControlCenterService disconnection error");
            }
        }

        private async Task ReconnectAsync(ControlCenterServiceSettings ccsSettings, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Reconnecting to ControlCenterService");
            await Task.Delay(ReconnectDelayInMilliseconds, cancellationToken);
            await InitializeAsync(ccsSettings, cancellationToken);
        }
    }
}