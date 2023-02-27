using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Domain.Models.Configuration;

namespace EventHandler.Application.Contracts.Infrastructure.Factory
{
    public interface ISocketListenerFactory
    {
        ISocketListener GetSocketListener(EventHandlerRawSocket socket);
    }
}
