using EventHandler.Application.Handlers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Xunit;

namespace EventHandler.UnitTests.Application.Handlers
{
    public class EndOfFileCharacterHandlerTests
    {
        private readonly Mock<ILogger> _mockLogger = new Mock<ILogger>();
        private readonly string _buffer = string.Empty;
        private readonly string[] _eofCharacters = new string[] { "/Data>", "***" };

        [Fact]
        public void EofCharacterHandle_BufferNewValue_ShouldBeEmpty()
        {
            //Arrange
            const string Message = "<Data> new data </Data>";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, _buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            result.BufferNewValue.Should().BeEmpty();
        }

        [Fact]
        public void EofCharacterHandle_BufferNewValue_ShouldBeEqual()
        {
            //Arrange
            const string Message = "<Data> test data";
            const string ExpectedResult = "<Data> test data";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, _buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            result.BufferNewValue.Should().BeEquivalentTo(ExpectedResult);
        }

        [Fact]
        public void EofCharacterHandle_CompletedMessages_ShouldBeOne()
        {
            //Arrange
            const string Message = "test end </Data>";
            const string Buffer = "<Data> test data";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, Buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            result.CompletedMessages.Should().HaveCount(1);
        }

        [Fact]
        public void EofCharacterHandle_CompletedMessages_ShouldBeTwo()
        {
            //Arrange
            const string Message = "<Data>cos</Data><Data>cos2></Data>";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, _buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            result.CompletedMessages.Should().HaveCount(2);
        }

        [Fact]
        public void EofCharacterHandle_CompletedMessages_ShouldBeTwoWithExistingBuffer()
        {
            //Arrange
            const string Message = "data</Data><Data>Other example</Data>";
            const string Buffer = "<Data>Example";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, Buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            using (new AssertionScope())
            {
                result.CompletedMessages.Should().HaveCount(2);
                result.BufferNewValue.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public void EofCharacterHandle_CompletedMessages_ShouldBeTwoWithNewBuffer()
        {
            //Arrange
            const string Message = "<Data>Smth</Data>Alive message***<Data>New start";
            const string ExpectedResult = "<Data>New start";
            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, _buffer, _eofCharacters, Message);

            //Act
            var result = sut.ReturnEofDetectionResult();

            //Assert
            using (new AssertionScope())
            {
                result.CompletedMessages.Should().HaveCount(2);
                result.BufferNewValue.Should().BeEquivalentTo(ExpectedResult);
            }
        }

        [Fact]
        public void EofCharacterHandle_BufferNewValue_ShouldJoinMessages()
        {
            //Arrange
            const string Buffer = "<Data>New start";
            const string Message = " of data will be more";
            const string ExpectedMessagesResult = "<Data>New start of data will be more some more data before end</Data>";
            const string ExpectedBufferResult = "<Dat";

            var sut = new EndOfFileCharacterHandler(_mockLogger.Object, Buffer, _eofCharacters, Message);

            var buffer2 = sut.ReturnEofDetectionResult().BufferNewValue;
            const string Message2 = " some more data before end";

            var sut2 = new EndOfFileCharacterHandler(_mockLogger.Object, buffer2, _eofCharacters, Message2);

            var buffer3 = sut2.ReturnEofDetectionResult().BufferNewValue;
            const string Message3 = "</Data><Dat";

            var sut3 = new EndOfFileCharacterHandler(_mockLogger.Object, buffer3, _eofCharacters, Message3);

            //Act
            var result = sut3.ReturnEofDetectionResult();

            //Assert
            using (new AssertionScope())
            {
                result.CompletedMessages.Should().HaveCount(1);
                result.CompletedMessages.First().Should().BeEquivalentTo(ExpectedMessagesResult);
                result.BufferNewValue.Should().BeEquivalentTo(ExpectedBufferResult);
            }
        }
    }
}