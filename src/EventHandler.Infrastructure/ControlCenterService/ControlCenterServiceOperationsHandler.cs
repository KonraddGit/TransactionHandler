using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ControlCenterService
{
    public class ControlCenterServiceOperationsHandler : IControlCenterServiceOperationsHandler
    {
        private readonly ILogger _logger;
        private readonly IControlCenterServiceConnector _ccsConnector;

        public ControlCenterServiceOperationsHandler(
            ILogger<ControlCenterServiceOperationsHandler> logger,
            IControlCenterServiceConnector ccsConnector)
        {
            _logger = logger;
            _ccsConnector = ccsConnector;
        }

        public async Task StartCCSConnectionAsync(ControlCenterServiceSettings ccsSettings, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Control center service configuration started");

            try
            {
                await _ccsConnector.InitializeAsync(ccsSettings, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error occurred on control center service initialization procedure");
            }
        }

        public void StopCCSConnection()
        {
            if (_ccsConnector != null)
            {
                if (_ccsConnector.IsConnected)
                {
                    _logger.LogInformation("Shutting down control center service connection");

                    _ccsConnector.Stop();
                }
            }

            _logger.LogInformation("Control center service stopped");
        }
    }
}
