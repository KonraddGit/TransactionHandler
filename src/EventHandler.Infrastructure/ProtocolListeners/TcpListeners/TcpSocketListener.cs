using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.ProtocolListeners.TcpListeners
{
    public class TcpSocketListener : ISocketListener
    {
        private const int MaxConcurrency = 5;

        private readonly ILogger _logger;
        private readonly IMessageRepository _messageRepository;
        private readonly EventHandlerRawSocket _rawSocket;

        public TcpSocketListener(
            ILogger logger,
            IMessageRepository messageRepository,
            EventHandlerRawSocket rawSocket)
        {
            _logger = logger;
            _rawSocket = rawSocket;
            _messageRepository = messageRepository;
        }

        public async Task ListenAsync(CancellationToken cancellationToken)
        {
            var clientReader = new ClientReader(_logger, _messageRepository, _rawSocket);
            var tcpListener = new TcpListener(IPAddress.Any, _rawSocket.Port);

            tcpListener.Start();

            _logger.LogInformation(
                "Port: {Port} is now open with eof characters: {Eofs}",
                _rawSocket.Port,
                string.Join(", ", _rawSocket.EOFCharacters));

            try
            {
                if (tcpListener.Server != null)
                {
                    using var concurrencySemaphore = new SemaphoreSlim(MaxConcurrency);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await concurrencySemaphore.WaitAsync(cancellationToken);

                        _ = Task.Factory.StartNew(async () =>
                        {
                            try
                            {
                                var client = await tcpListener.AcceptTcpClientAsync();
                                await clientReader.ReadClientAsync(client, cancellationToken);
                            }
                            finally
                            {
                                concurrencySemaphore.Release();
                            }
                        }, cancellationToken);
                    }
                }
            }
            catch (SocketException se)
            {
                _logger.LogError(se, "Error while listening to TCP port");
            }
            finally
            {
                tcpListener.Stop();
            }
        }
    }
}