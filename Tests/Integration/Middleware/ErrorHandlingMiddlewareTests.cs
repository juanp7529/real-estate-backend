using NUnit.Framework;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using real_estate_api.Infrastructure.Middleware;
using real_estate_api.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace real_estate_api.Tests.Integration.Middleware
{
    [TestFixture]
    public class ErrorHandlingMiddlewareTests
    {
        private Mock<ILogger<ErrorHandlingMiddleware>> _mockLogger;
        private Mock<IHostEnvironment> _mockEnvironment;
        private DefaultHttpContext _httpContext;
        private MemoryStream _responseStream;

        [SetUp]
   public void Setup()
  {
      _mockLogger = new Mock<ILogger<ErrorHandlingMiddleware>>();
_mockEnvironment = new Mock<IHostEnvironment>();
            _httpContext = new DefaultHttpContext();
         _responseStream = new MemoryStream();
            _httpContext.Response.Body = _responseStream;
 }

        [TearDown]
        public void TearDown()
        {
      _responseStream?.Dispose();
     }

#region NotFoundException Tests

     [Test]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_Returns404()
        {
          // Arrange
   _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            
            RequestDelegate next = (HttpContext hc) => throw new NotFoundException("Property", "123");

            var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            await middleware.InvokeAsync(_httpContext);

     // Assert
     _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            _httpContext.Response.ContentType.Should().Be("application/json");
        }

        [Test]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ReturnsCorrectErrorResponse()
        {
   // Arrange
        _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            var propertyId = "507f1f77bcf86cd799439011";
   
         RequestDelegate next = (HttpContext hc) => throw new NotFoundException("Property", propertyId);
       
            var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

// Act
      await middleware.InvokeAsync(_httpContext);

      // Assert
            _responseStream.Position = 0;
       var reader = new StreamReader(_responseStream);
      var responseBody = await reader.ReadToEndAsync();
        
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
      {
          PropertyNameCaseInsensitive = true
            });

   errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be(404);
    errorResponse.Type.Should().Be("Not Found");
    errorResponse.Message.Should().Contain(propertyId);
        errorResponse.TraceId.Should().NotBeNullOrEmpty();
        errorResponse.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
  }

        #endregion

        #region ValidationException Tests

        [Test]
        public async Task InvokeAsync_WhenValidationExceptionThrown_Returns400()
        {
            // Arrange
     _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            
  RequestDelegate next = (HttpContext hc) => throw new ValidationException("Id", "Invalid ID format");
 
     var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

       // Act
    await middleware.InvokeAsync(_httpContext);

    // Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task InvokeAsync_WhenValidationExceptionThrown_ReturnsErrorsDict()
        {
   // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
          var field = "MinPrice";
    var error = "Price cannot be negative";

            RequestDelegate next = (HttpContext hc) => throw new ValidationException(field, error);
            
            var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

            // Act
            await middleware.InvokeAsync(_httpContext);

    // Assert
        _responseStream.Position = 0;
        var reader = new StreamReader(_responseStream);
            var responseBody = await reader.ReadToEndAsync();
         
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
   {
    PropertyNameCaseInsensitive = true
      });

            errorResponse.Should().NotBeNull();
            errorResponse!.StatusCode.Should().Be(400);
            errorResponse.Type.Should().Be("Validation Error");
            errorResponse.Errors.Should().NotBeNull();
   errorResponse.Errors.Should().ContainKey(field);
        }

        [Test]
        public async Task InvokeAsync_WhenValidationExceptionWithMultipleErrors_ReturnsAllErrors()
        {
            // Arrange
     _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
  var errors = new Dictionary<string, string[]>
         {
         { "MinPrice", new[] { "Cannot be negative" } },
  { "MaxPrice", new[] { "Cannot be negative" } }
        };
            
   RequestDelegate next = (HttpContext hc) => throw new ValidationException(errors);
            
         var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

     // Act
      await middleware.InvokeAsync(_httpContext);

            // Assert
        _responseStream.Position = 0;
    var reader = new StreamReader(_responseStream);
 var responseBody = await reader.ReadToEndAsync();
  
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
            {
        PropertyNameCaseInsensitive = true
            });

     errorResponse.Should().NotBeNull();
      errorResponse!.Errors.Should().HaveCount(2);
            errorResponse.Errors.Should().ContainKeys("MinPrice", "MaxPrice");
        }

        #endregion

 #region BusinessException Tests

        [Test]
        public async Task InvokeAsync_WhenBusinessExceptionThrown_Returns422()
        {
// Arrange
      _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
         
          RequestDelegate next = (HttpContext hc) => throw new BusinessException("Business rule violated");
            
            var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

   // Act
await middleware.InvokeAsync(_httpContext);

      // Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.UnprocessableEntity);
        }

        [Test]
        public async Task InvokeAsync_WhenBusinessExceptionThrown_ReturnsCorrectType()
   {
          // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
     
            RequestDelegate next = (HttpContext hc) => throw new BusinessException("Cannot process request");
            
 var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

      // Act
     await middleware.InvokeAsync(_httpContext);

            // Assert
 _responseStream.Position = 0;
      var reader = new StreamReader(_responseStream);
    var responseBody = await reader.ReadToEndAsync();
     
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
            {
PropertyNameCaseInsensitive = true
          });

            errorResponse.Should().NotBeNull();
          errorResponse!.StatusCode.Should().Be(422);
            errorResponse.Type.Should().Be("Business Rule Violation");
        }

      #endregion

   #region DatabaseException Tests

     [Test]
     public async Task InvokeAsync_WhenDatabaseExceptionThrown_Returns503()
  {
            // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
   
      RequestDelegate next = (HttpContext hc) => throw new DatabaseException("Database connection failed");
            
 var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

        // Act
     await middleware.InvokeAsync(_httpContext);

// Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
        }

  [Test]
  public async Task InvokeAsync_WhenDatabaseExceptionThrown_HidesInternalDetails()
        {
         // Arrange
         _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            var internalMessage = "MongoDB connection string: mongodb://secret@localhost";
            
  RequestDelegate next = (HttpContext hc) => throw new DatabaseException(internalMessage);
            
 var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

 // Act
   await middleware.InvokeAsync(_httpContext);

         // Assert
            _responseStream.Position = 0;
            var reader = new StreamReader(_responseStream);
            var responseBody = await reader.ReadToEndAsync();
            
  var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
     PropertyNameCaseInsensitive = true
          });

       errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().NotContain("MongoDB");
   errorResponse.Message.Should().NotContain("secret");
            errorResponse.Message.Should().Be("A database error occurred. Please try again later.");
        }

  #endregion

    #region Generic Exception Tests

        [Test]
        public async Task InvokeAsync_WhenGenericExceptionThrown_Returns500()
        {
    // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
         
 RequestDelegate next = (HttpContext hc) => throw new Exception("Unexpected error");
   
     var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

// Act
       await middleware.InvokeAsync(_httpContext);

   // Assert
            _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Test]
      public async Task InvokeAsync_WhenGenericExceptionThrown_HidesInternalDetails()
        {
            // Arrange
   _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        var internalMessage = "Internal error with sensitive data";
        
     RequestDelegate next = (HttpContext hc) => throw new Exception(internalMessage);
            
    var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

    // Act
 await middleware.InvokeAsync(_httpContext);

    // Assert
         _responseStream.Position = 0;
            var reader = new StreamReader(_responseStream);
            var responseBody = await reader.ReadToEndAsync();
        
   var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
      {
     PropertyNameCaseInsensitive = true
            });

            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().NotContain("sensitive data");
         errorResponse.Message.Should().Be("An unexpected error occurred. Please try again later.");
        }

      #endregion

  #region Development Environment Tests

        [Test]
   public async Task InvokeAsync_InDevelopment_IncludesStackTrace()
   {
     // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Development");
            
      RequestDelegate next = (HttpContext hc) => throw new NotFoundException("Property", "123");
     
        var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

          // Act
  await middleware.InvokeAsync(_httpContext);

  // Assert
          _responseStream.Position = 0;
            var reader = new StreamReader(_responseStream);
         var responseBody = await reader.ReadToEndAsync();
     
          var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
 {
                PropertyNameCaseInsensitive = true
      });

            errorResponse.Should().NotBeNull();
            errorResponse!.StackTrace.Should().NotBeNullOrEmpty();
            errorResponse.Details.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task InvokeAsync_InProduction_DoesNotIncludeStackTrace()
        {
            // Arrange
 _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
    
            RequestDelegate next = (HttpContext hc) => throw new NotFoundException("Property", "123");
          
  var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

  // Act
       await middleware.InvokeAsync(_httpContext);

    // Assert
     _responseStream.Position = 0;
         var reader = new StreamReader(_responseStream);
   var responseBody = await reader.ReadToEndAsync();
      
       var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
     {
   PropertyNameCaseInsensitive = true
        });

            errorResponse.Should().NotBeNull();
     errorResponse!.StackTrace.Should().BeNullOrEmpty();
            errorResponse.Details.Should().BeNullOrEmpty();
        }

      #endregion

        #region Success Path Tests

        [Test]
public async Task InvokeAsync_WhenNoExceptionThrown_ContinuesPipeline()
        {
            // Arrange
  _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
            var nextCalled = false;
            
            RequestDelegate next = (HttpContext hc) =>
  {
                nextCalled = true;
  return Task.CompletedTask;
   };
  
            var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

   // Act
   await middleware.InvokeAsync(_httpContext);

 // Assert
            nextCalled.Should().BeTrue();
   _httpContext.Response.StatusCode.Should().Be(200); // Default success status
        }

   #endregion

        #region TraceId Tests

        [Test]
      public async Task InvokeAsync_WhenExceptionThrown_IncludesTraceId()
        {
     // Arrange
            _mockEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
     var expectedTraceId = "test-trace-id-12345";
            _httpContext.TraceIdentifier = expectedTraceId;
          
 RequestDelegate next = (HttpContext hc) => throw new NotFoundException("Property", "123");
     
         var middleware = new ErrorHandlingMiddleware(next, _mockLogger.Object, _mockEnvironment.Object);

            // Act
        await middleware.InvokeAsync(_httpContext);

       // Assert
       _responseStream.Position = 0;
      var reader = new StreamReader(_responseStream);
         var responseBody = await reader.ReadToEndAsync();
            
     var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
            });

  errorResponse.Should().NotBeNull();
         errorResponse!.TraceId.Should().Be(expectedTraceId);
        }

        #endregion
    }

    // Helper class to deserialize error responses in tests
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Details { get; set; }
        public string TraceId { get; set; } = null!;
      public DateTime Timestamp { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? StackTrace { get; set; }
  }
}
