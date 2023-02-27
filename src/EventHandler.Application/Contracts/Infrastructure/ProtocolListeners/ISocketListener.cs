using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.ProtocolListeners
{
    public interface ISocketListener
    {
        Task ListenAsync(CancellationToken cancellationToken);
    }
}
