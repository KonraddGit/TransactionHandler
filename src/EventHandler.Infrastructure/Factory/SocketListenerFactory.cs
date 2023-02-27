using EventHandler.Application.Contracts.Infrastructure.Factory;
using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Configuration.Enums;
using EventHandler.Infrastructure.ProtocolListeners;
using EventHandler.Infrastructure.ProtocolListeners.TcpListeners;
using Microsoft.Extensions.Logging;
using System;

namespace EventHandler.Infrastructure.Factory
{
    public class SocketListenerFactory : ISocketListenerFactory
    {
        private readonly ILogger _logger;
        private readonly IMessageRepository _messageRepository;

        public SocketListenerFactory(
            ILogger<SocketListenerFactory> logger,
            IMessageRepository messageRepository)
        {
            _logger = logger;
            _messageRepository = messageRepository;
        }

        public ISocketListener GetSocketListener(EventHandlerRawSocket socket)
            => (socket.ProtocolType, socket.ClientMode) switch
            {
                (ProtocolType.Tcp, true) => new ClientModeTcpListener(_logger, _messageRepository, socket),
                (ProtocolType.Tcp, false) => new TcpSocketListener(_logger, _messageRepository, socket),
                (ProtocolType.Udp, _) => new UdpSocketListener(_logger, _messageRepository, socket),
                _ => throw new NotSupportedException($"Listener of {socket.ProtocolType} protocol is not supported")
            };
    }
}
