using FluentAssertions;
using NUnit.Framework;
using real_estate_api.Application.Exceptions;

namespace real_estate_api.Tests.Unit.Exceptions
{
    [TestFixture]
    public class NotFoundExceptionTests
    {
        [Test]
        public void Constructor_WithMessage_SetsMessageCorrectly()
        {
            // Arrange
            var message = "Resource not found";

            // Act
            var exception = new NotFoundException(message);

            // Assert
            exception.Message.Should().Be(message);
        }

        [Test]
        public void Constructor_WithNameAndKey_GeneratesCorrectMessage()
        {
            // Arrange
            var name = "Property";
            var key = "507f1f77bcf86cd799439011";

            // Act
            var exception = new NotFoundException(name, key);

            // Assert
            exception.Message.Should().Be($"{name} with id '{key}' was not found.");
        }

        [Test]
        public void Constructor_WithNameAndKey_IsInstanceOfException()
        {
            // Arrange & Act
            var exception = new NotFoundException("Property", "123");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Test]
        public void Constructor_WithDifferentTypes_HandlesObjectKey()
        {
            // Arrange
            var name = "Owner";
            var numericKey = 12345;

            // Act
            var exception = new NotFoundException(name, numericKey);

            // Assert
            exception.Message.Should().Contain("12345");
        }
    }
}
