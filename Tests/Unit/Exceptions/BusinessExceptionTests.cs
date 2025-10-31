using FluentAssertions;
using NUnit.Framework;
using real_estate_api.Application.Exceptions;

namespace real_estate_api.Tests.Unit.Exceptions
{
    [TestFixture]
    public class BusinessExceptionTests
    {
        [Test]
        public void Constructor_WithMessage_SetsMessageCorrectly()
        {
            // Arrange
            var message = "Business rule violation occurred";

            // Act
            var exception = new BusinessException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().BeNull();
        }

        [Test]
        public void Constructor_WithMessageAndInnerException_SetsPropertiesCorrectly()
        {
            // Arrange
            var message = "Failed to process business logic";
            var innerException = new InvalidOperationException("Inner error");

            // Act
            var exception = new BusinessException(message, innerException);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().Be(innerException);
            exception.InnerException.Should().BeOfType<InvalidOperationException>();
        }

        [Test]
        public void Constructor_IsInstanceOfException()
        {
            // Arrange & Act
            var exception = new BusinessException("Test");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Test]
        public void Constructor_WithInnerException_PreservesInnerExceptionMessage()
        {
            // Arrange
            var innerMessage = "Original error message";
            var innerException = new Exception(innerMessage);
            var message = "Business exception wrapper";

            // Act
            var exception = new BusinessException(message, innerException);

            // Assert
            exception.InnerException!.Message.Should().Be(innerMessage);
        }

        [Test]
        public void Constructor_WithNullInnerException_DoesNotThrow()
        {
            // Arrange & Act
            var exception = new BusinessException("Test message", null!);

            // Assert
            exception.Should().NotBeNull();
            exception.InnerException.Should().BeNull();
        }
    }
}
