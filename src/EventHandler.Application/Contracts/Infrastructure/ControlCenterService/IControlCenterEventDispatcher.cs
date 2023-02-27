using System;

namespace EventHandler.Application.Contracts.Infrastructure.ControlCenterService
{
    public interface IControlCenterEventDispatcher
    {
        void DispatchState(ControlCenterServiceInterface sender, EventArgs args);
        void DispatchAction(MessageProtocol message, ControlCenterServiceInterface sender);
        void DispatchError(Exception ex, ControlCenterServiceInterface sender);
        void DispatchDiagnosticsInfo(string message, ControlCenterServiceInterface sender);
    }
}