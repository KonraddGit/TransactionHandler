using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Configuration.Enums;
using EventHandler.Infrastructure.Factory;
using EventHandler.Infrastructure.ProtocolListeners;
using EventHandler.Infrastructure.ProtocolListeners.TcpListeners;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace EventHandler.UnitTests.Infrastructure.Factory
{
    public class SocketListenerFactoryTests
    {
        private readonly SocketListenerFactory _sut;

        public SocketListenerFactoryTests()
        {
            var mockLogger = new Mock<ILogger<SocketListenerFactory>>();
            var mockMessageRepository = new Mock<IMessageRepository>();
            _sut = new SocketListenerFactory(mockLogger.Object, mockMessageRepository.Object);
        }

        [Fact]
        public void ListenSocketFactory_WhenTCP_ShouldBeOfTypeTCP()
        {
            //Arrange
            var socket = new EventHandlerRawSocket
            {
                ProtocolType = ProtocolType.Tcp
            };

            //Act
            var socketListener = _sut.GetSocketListener(socket);

            //Assert
            socketListener.Should().BeOfType<TcpSocketListener>();
        }

        [Fact]
        public void ListenSocketFactory_WhenUDP_ShouldBeOfTypeUDP()
        {
            //Arrange
            var socket = new EventHandlerRawSocket
            {
                ProtocolType = ProtocolType.Udp
            };

            //Act
            var socketListener = _sut.GetSocketListener(socket);

            //Assert
            socketListener.Should().BeOfType<UdpSocketListener>();
        }

        [Fact]
        public void ListenSocketFactory_WhenNotSupported_ShoudlThrowAnException()
        {
            //Arrange
            var socket = new EventHandlerRawSocket
            {
                ProtocolType = (ProtocolType)(-1)
            };

            //Assert
            Assert.Throws<NotSupportedException>(() => _sut.GetSocketListener(socket));
        }
    }
}
