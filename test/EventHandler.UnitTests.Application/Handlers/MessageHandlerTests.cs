using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Handlers;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Configuration.Enums;
using EventHandler.Domain.Models.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace EventHandler.UnitTests.Application.Handlers
{
    public class MessageHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private readonly Mock<IMessageRepository> _mockMessageRepository = new Mock<IMessageRepository>();

        [Fact]
        public async Task HandleAsync_WhenHandleImmediatelyHasTwoRecords_ShouldCallHandleImmediatelyAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "hello***";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmidiatelySerialize = "False",
                HandleImmediatelySerialize = "True",
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenHandleImmediatelyHasTwoRecords_ShouldNotCallHandleImmediatelyAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "hello***";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmidiatelySerialize = "True",
                HandleImmediatelySerialize = "False",
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenHandleImmediately_ShouldCallSaveMessageAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "hello***";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmediatelySerialize = "True",
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenHandleImmediately_ShouldNotCallSaveMessageAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "testingrandommessage";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmediatelySerialize = "False",
                BufferLength = 10000,
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenBufferOverflow_ShouldCallSaveMessageAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "bufferoverflowcase";
            var messageType = SaveMessageType.BufferOverflow;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmediatelySerialize = "False",
                BufferLength = 1,
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenBufferNotOverflow_ShouldNotCallSaveMessageAsync()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "buff";
            var messageType = SaveMessageType.BufferOverflow;

            var rawSocket = new EventHandlerRawSocket
            {
                HandleImmediatelySerialize = "False",
                BufferLength = 5,
                EOFCharacters = new string[] { "/Data>", "***" }
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Never);
        }

        [Fact]
        public async Task HandleAsync_WhenClientRecognition_ShouldBeFull()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "test";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                ClientRecognitionType = ClientRecognitionType.Full,
                HandleImmediatelySerialize = "True"
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, It.Is<string>(x => x.Contains("127.0.0.1:5112")), messageType));
        }

        [Fact]
        public async Task HandleAsync_WhenClientRecognition_ShouldBeIPAddress()
        {
            //Arrange
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            const string Message = "test";
            var messageType = SaveMessageType.HandleImmediately;

            var rawSocket = new EventHandlerRawSocket
            {
                ClientRecognitionType = ClientRecognitionType.IPAddress,
                HandleImmediatelySerialize = "True"
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, It.Is<string>(x => x.Contains("127.0.0.1")), messageType));
        }

        [Fact]
        public async Task HandleAsync_WhenBatchesFound_ShouldBeSavedOnce()
        {
            //Arrange
            const string Message = "hello***";
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);
            var messageType = SaveMessageType.EndOfFileCharacter;

            var rawSocket = new EventHandlerRawSocket
            {
                EOFCharacters = new string[] { "***" },
                BufferLength = 10000,
                HandleImmediatelySerialize = "False"
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository
                .Verify(x => x.SaveMessageAsync(rawSocket, Message, endPoint.ToString(), messageType), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenBatchesFound_ShouldBeSavedExactlyThreeTimes()
        {
            //Arrange
            const string Message = "hello***hello***hello***";
            const int ExpectedResult = 3;
            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5112);

            var rawSocket = new EventHandlerRawSocket
            {
                EOFCharacters = new string[] { "***" },
                BufferLength = 10000,
                HandleImmediatelySerialize = "False"
            };

            var sut = new MessageHandler(_mockLogger.Object, _mockMessageRepository.Object, rawSocket);

            //Act
            await sut.HandleAsync(Message, endPoint);

            //Assert
            _mockMessageRepository.Invocations.Count.Should().Be(ExpectedResult);
        }
    }
}
