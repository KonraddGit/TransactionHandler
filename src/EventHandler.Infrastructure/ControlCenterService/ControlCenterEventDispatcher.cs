using EventHandler.Application.Contracts.Infrastructure.ControlCenterService;
using Microsoft.Extensions.Logging;
using System;

namespace EventHandler.Infrastructure.ControlCenterService
{
    public class ControlCenterEventDispatcher : IControlCenterEventDispatcher
    {
        private readonly ILogger _logger;

        public ControlCenterEventDispatcher(
            ILogger<ControlCenterEventDispatcher> logger)
        {
            _logger = logger;
        }

        public void DispatchState(ControlCenterServiceInterface sender, EventArgs args)
        {
            _logger.LogInformation("State of ControlCenterService changed");
        }

        public void DispatchAction(MessageProtocol message, ControlCenterServiceInterface sender)
        {
            _logger.LogInformation("Dispatching command, {Message}", message.MessageSubject);
        }

        public void DispatchDiagnosticsInfo(string message, ControlCenterServiceInterface sender)
        {
            _logger.LogTrace("Diagnostics, {Message}", message);
        }

        public void DispatchError(Exception ex, ControlCenterServiceInterface sender)
        {
            _logger.LogError(ex, "ControlCenterService connection error");
        }
    }
}
