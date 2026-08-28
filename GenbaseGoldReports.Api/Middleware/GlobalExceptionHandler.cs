using GenbaseGoldReports.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);

            var (statusCode, title, detail, errors) = MapException(exception);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            if (errors != null)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static (int StatusCode, string Title, string Detail, IDictionary<string, string[]>? Errors) MapException(Exception exception)
        {
            return exception switch
            {
                ValidationException valEx => (
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    valEx.Message,
                    valEx.Errors
                ),
                NotFoundException notFoundEx => (
                    StatusCodes.Status404NotFound,
                    "Resource Not Found",
                    notFoundEx.Message,
                    null
                ),
                ForbiddenAccessException forbiddenEx => (
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    forbiddenEx.Message,
                    null
                ),
                UnauthorizedAccessException => (
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Authentication credentials are missing or invalid.",
                    null
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    null
                )
            };
        }
    }
}
