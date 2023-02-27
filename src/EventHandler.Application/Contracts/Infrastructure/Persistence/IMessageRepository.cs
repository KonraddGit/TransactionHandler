using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Persistence;
using System.Threading.Tasks;

namespace EventHandler.Application.Contracts.Infrastructure.Persistence
{
    public interface IMessageRepository
    {
        Task SaveMessageAsync(EventHandlerRawSocket rawSocket, string message, string endPoint, SaveMessageType saveMessageType);
    }
}
