using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ProtocolListeners.TcpListeners
{
    public class ClientModeTcpListener : ISocketListener
    {
        private readonly ILogger _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly EventHandlerRawSocket _rawSocket;

        public ClientModeTcpListener(
            ILogger logger,
            IMessageRepository messageRepository,
            EventHandlerRawSocket rawSocket)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _rawSocket = rawSocket;
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            var clientReader = new ClientReader(_logger, _messageRepository, _rawSocket);
            var client = new TcpClient();

            await client.ConnectAsync(
                 _rawSocket.IpAddress,
                 _rawSocket.Port);

            _logger.LogInformation(
                "Port: {Port} is now open with eof characters: {Eofs}",
                _rawSocket.Port,
                string.Join(", ", _rawSocket.EOFCharacters));

            try
            {
                await clientReader.ReadClientAsync(client, cancellationToken);
            }
            catch (SocketException se)
            {
                _logger.LogError(se, "Error while listening to TCP port");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
