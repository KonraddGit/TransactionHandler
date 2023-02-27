using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Application.Handlers;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ProtocolListeners
{
    public class UdpSocketListener : ISocketListener
    {
        private readonly ILogger _logger;
        private readonly EventHandlerRawSocket _rawSocket;
        private readonly MessageHandler _messageHandler;

        public UdpSocketListener(
            ILogger logger,
            IMessageRepository messageRepository,
            EventHandlerRawSocket rawSocket)
        {
            _logger = logger;
            _rawSocket = rawSocket;
            _messageHandler = new MessageHandler(_logger, messageRepository, _rawSocket);
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            var listener = new UdpClient(_rawSocket.Port);

            _logger.LogInformation
                ("Port: {Port} is now open with eof characters: {Eofs}", _rawSocket.Port, string.Join(", ", _rawSocket.EOFCharacters));

            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var bytes = await listener.ReceiveAsync();
                    var remoteEndPoint = bytes.RemoteEndPoint;
                    var data = Encoding.ASCII.GetString(bytes.Buffer);

                    _logger.LogInformation("Data received | Port={Port}, Length={Length}, Source={ClientEndPoint}",
                    _rawSocket.Port, data.Length, remoteEndPoint);
                    _logger.LogTrace("Received data body: {data}", data);

                    await _messageHandler.HandleAsync(data, remoteEndPoint);

                    if (_rawSocket.CutClientAfterFirstFrame)
                    {
                        _logger.LogInformation("Cutting client after first frame");
                        listener.Close();
                    }
                }
            }
            catch (SocketException se)
            {
                _logger.LogError(se, "Error while listening to UDP port");
            }
            finally
            {
                listener.Close();
            }
        }
    }
}
