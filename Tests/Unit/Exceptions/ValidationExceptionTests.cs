using FluentAssertions;
using NUnit.Framework;
using real_estate_api.Application.Exceptions;

namespace real_estate_api.Tests.Unit.Exceptions
{
    [TestFixture]
    public class ValidationExceptionTests
    {
        [Test]
        public void Constructor_WithMessage_SetsMessageCorrectly()
        {
            // Arrange
            var message = "Validation failed";

            // Act
            var exception = new ValidationException(message);

            // Assert
            exception.Message.Should().Be(message);
            exception.Errors.Should().NotBeNull();
            exception.Errors.Should().BeEmpty();
        }

        [Test]
        public void Constructor_WithFieldAndError_SetsErrorsDictionary()
        {
            // Arrange
            var field = "Id";
            var error = "Invalid property ID format";

            // Act
            var exception = new ValidationException(field, error);

            // Assert
            exception.Message.Should().Contain(field);
            exception.Errors.Should().ContainKey(field);
            exception.Errors[field].Should().Contain(error);
            exception.Errors[field].Should().HaveCount(1);
        }

        [Test]
        public void Constructor_WithErrorsDictionary_SetsErrorsCorrectly()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
   {
      { "Name", new[] { "Name is required", "Name must be at least 3 characters" } },
 { "Price", new[] { "Price must be positive" } }
        };

            // Act
            var exception = new ValidationException(errors);

            // Assert
            exception.Message.Should().Be("One or more validation errors occurred.");
            exception.Errors.Should().HaveCount(2);
            exception.Errors["Name"].Should().HaveCount(2);
            exception.Errors["Price"].Should().HaveCount(1);
        }

        [Test]
        public void Constructor_WithFieldAndError_GeneratesCorrectMessage()
        {
            // Arrange
            var field = "Email";
            var error = "Invalid email format";

            // Act
            var exception = new ValidationException(field, error);

            // Assert
            exception.Message.Should().Be($"Validation error in field '{field}'.");
        }

        [Test]
        public void Constructor_IsInstanceOfException()
        {
            // Arrange & Act
            var exception = new ValidationException("Test error");

            // Assert
            exception.Should().BeAssignableTo<Exception>();
        }

        [Test]
        public void Constructor_WithMultipleFieldErrors_StoresAllErrors()
        {
            // Arrange
            var errors = new Dictionary<string, string[]>
            {
      { "MinPrice", new[] { "Min price cannot be negative" } },
    { "MaxPrice", new[] { "Max price cannot be negative" } },
   { "Price", new[] { "Min price cannot be greater than max price" } }
          };

            // Act
            var exception = new ValidationException(errors);

            // Assert
            exception.Errors.Should().HaveCount(3);
            exception.Errors.Keys.Should().Contain(new[] { "MinPrice", "MaxPrice", "Price" });
        }

        [Test]
        public void Constructor_WithEmptyField_StillCreatesException()
        {
            // Arrange
            var field = "";
            var error = "Some error";

            // Act
            var exception = new ValidationException(field, error);

            // Assert
            exception.Should().NotBeNull();
            exception.Errors.Should().ContainKey(field);
        }
    }
}
