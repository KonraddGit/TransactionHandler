using EventHandler.Application.Contracts.Infrastructure.Factory;
using EventHandler.Application.Handlers;
using EventHandler.Domain.Models.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventHandler.UnitTests.Application.Handlers
{
    public class OpenPortHandlerTests
    {
        [Fact]
        public async Task InvokeAsync_WhenCalled_ShouldBeCalledOnce()
        {
            //Arrange
            var cancellationToken = new CancellationToken();
            var mockLogger = new Mock<ILogger>();
            var mockSocketListener = new Mock<ISocketListenerFactory>();
            var mockRawSockets = new EventHandlerRawListOfSockets
            {
                Sockets = new List<EventHandlerRawSocket> { new EventHandlerRawSocket { CustomName = "Test", SocketConfigurationEnabled = true } }
            };
            var sut = new OpenPortHandler(mockLogger.Object, mockSocketListener.Object, mockRawSockets);

            //Act
            await sut.InvokeAsync(cancellationToken);

            //Assert
            mockSocketListener.Invocations.Count.Should().Be(1);
        }
    }
}
