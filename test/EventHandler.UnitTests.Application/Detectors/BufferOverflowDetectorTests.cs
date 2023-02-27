using EventHandler.Application.Detectors;
using Xunit;

namespace EventHandler.UnitTests.Application.Detectors
{
    public class BufferOverflowDetectorTests
    {
        [Fact]
        public void BufferOverflow_WhenDetected_ShouldReturnTrue()
        {
            //Arrange
            const string Buffer = "testingoverflow";
            const int BufferLenght = 5;

            //Act
            var result = BufferOverflowDetector.Detect(Buffer, BufferLenght);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BufferOverflow_WhenNotDetected_ShouldReturnFalse()
        {
            //Arrange
            const string Buffer = "test";
            const int BufferLenght = 5;

            //Act
            var result = BufferOverflowDetector.Detect(Buffer, BufferLenght);

            //Assert
            result.Should().BeFalse();
        }
    }
}
