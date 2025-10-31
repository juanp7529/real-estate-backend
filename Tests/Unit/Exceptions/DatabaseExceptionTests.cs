using FluentAssertions;
using NUnit.Framework;
using real_estate_api.Application.Exceptions;

namespace real_estate_api.Tests.Unit.Exceptions
{
    [TestFixture]
    public class DatabaseExceptionTests
    {
        [Test]
        public void Constructor_WithMessage_SetsMessageCorrectly()
        {
            // Arrange
            var message = "Database connection failed";

            // Act
            var exception = new DatabaseException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().BeNull();
        }

        [Test]
        public void Constructor_WithMessageAndInnerException_SetsPropertiesCorrectly()
        {
            // Arrange
            var message = "Failed to retrieve data from database";
            var innerException = new TimeoutException("Connection timeout");

            // Act
            var exception = new DatabaseException(message, innerException);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().Be(innerException);
            exception.InnerException.Should().BeOfType<TimeoutException>();
        }

        [Test]
        public void Constructor_IsInstanceOfException()
        {
            // Arrange & Act
            var exception = new DatabaseException("Test");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Test]
        public void Constructor_WithInnerException_PreservesInnerExceptionMessage()
        {
            // Arrange
            var innerMessage = "MongoDB connection refused";
            var innerException = new Exception(innerMessage);
            var message = "Database exception wrapper";

            // Act
            var exception = new DatabaseException(message, innerException);

            // Assert
            exception.InnerException!.Message.Should().Be(innerMessage);
        }

        [Test]
        public void Constructor_WithNullInnerException_DoesNotThrow()
        {
            // Arrange & Act
            var exception = new DatabaseException("Test message", null!);

            // Assert
            exception.Should().NotBeNull();
            exception.InnerException.Should().BeNull();
        }

        [Test]
        public void Constructor_WithDatabaseSpecificException_WrapsCorrectly()
        {
            // Arrange
            var mongoException = new Exception("MongoDB: Connection pool timeout");
            var message = "Failed to connect to MongoDB";

            // Act
            var exception = new DatabaseException(message, mongoException);

            // Assert
            exception.Message.Should().Be(message);
            exception.InnerException.Should().NotBeNull();
            exception.InnerException!.Message.Should().Contain("MongoDB");
        }
    }
}
