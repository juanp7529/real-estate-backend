using real_estate_api.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace real_estate_api.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier,
                Timestamp = DateTime.UtcNow
            };

            switch (exception)
            {
                case NotFoundException notFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = notFoundException.Message;
                    errorResponse.Type = "Not Found";
                    _logger.LogWarning(notFoundException, "Resource not found: {Message}", notFoundException.Message);
                    break;

                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = validationException.Message;
                    errorResponse.Type = "Validation Error";
                    errorResponse.Errors = validationException.Errors;
                    _logger.LogWarning(validationException, "Validation error: {Message}", validationException.Message);
                    break;

                case BusinessException businessException:
                    response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = businessException.Message;
                    errorResponse.Type = "Business Rule Violation";
                    _logger.LogWarning(businessException, "Business rule violation: {Message}", businessException.Message);
                    break;

                case DatabaseException databaseException:
                    response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "A database error occurred. Please try again later.";
                    errorResponse.Type = "Database Error";
                    _logger.LogError(databaseException, "Database error: {Message}", databaseException.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "An unexpected error occurred. Please try again later.";
                    errorResponse.Type = "Internal Server Error";
                    _logger.LogError(exception, "Unexpected error: {Message}", exception.Message);
                    break;
            }

            // Include stack trace only in development
            if (_environment.IsDevelopment())
            {
                errorResponse.Details = exception.Message;
                errorResponse.StackTrace = exception.StackTrace;
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await response.WriteAsync(jsonResponse);
        }
    }

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
