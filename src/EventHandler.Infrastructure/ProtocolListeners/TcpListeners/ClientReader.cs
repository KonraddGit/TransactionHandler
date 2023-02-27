using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Handlers;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Persistence;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ProtocolListeners.TcpListeners
{
    public class ClientReader
    {
        private readonly ILogger _logger;
        private readonly EventHandlerRawSocket _rawSocket;
        private readonly MessageHandler _messageHandler;

        public ClientReader(
            ILogger logger,
            IMessageRepository messageRepository,
            EventHandlerRawSocket rawSocket)
        {
            _logger = logger;
            _rawSocket = rawSocket;
            _messageHandler = new MessageHandler(_logger, messageRepository, _rawSocket);
        }

        public async Task ReadClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            var bytes = new byte[client.ReceiveBufferSize];
            var stream = client.GetStream();
            int count;

            while ((count = await stream.ReadAsync(bytes, 0, bytes.Length, cancellationToken)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                var data = Encoding.ASCII.GetString(bytes, 0, count);

                _logger.LogInformation("Data received | Port={Port}, Length={Length}, Source={ClientEndPoint}",
                    _rawSocket.Port, data.Length, clientEndPoint);
                _logger.LogTrace("Received data body: {data}", data);

                var result = await _messageHandler.HandleAsync(data, clientEndPoint);

                if (_rawSocket.CutClientAfterFirstFrame && result == SaveMessageType.EndOfFileCharacter)
                {
                    _logger.LogInformation("Cutting client after first frame");
                    client.Close();
                }
            }

            client.Close();
        }
    }
}
