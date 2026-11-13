using GenbaseGoldReports.Application.Common.Exceptions;
using GenbaseGoldReports.Application.Common.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GenbaseGoldReports.Api.Middleware
{
    public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IWebHostEnvironment env) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            logger.LogError(exception, "Unhandled exception on {Path}: {Message}",
                httpContext.Request.Path, exception.Message);

            var (statusCode, response) = MapException(exception, env.IsDevelopment());

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }

        private static (int StatusCode, ApiResponse<object>) MapException(Exception exception, bool isDev)
        {
            // 1. Unwrap nested exceptions (Dapper, MediatR, or AggregateException)
            var actualException = exception;
            while (actualException.InnerException != null && actualException is not ValidationException)
            {
                actualException = actualException.InnerException;
            }

            // 2. SQL Schema / Missing Column Errors (207: Bad column, 208: Bad table)
            if (actualException is SqlException sqlEx && sqlEx.Number is 207 or 208)
            {
                return (
                    StatusCodes.Status422UnprocessableEntity,
                    ApiResponse<object>.Failure(
                        isDev
                            ? $"Database schema mismatch: {sqlEx.Message}"
                            : "A database configuration incompatibility was detected. Please contact technical support.",
                        "Database Configuration Error"
                    )
                );
            }

            return exception switch
            {
                ValidationException valEx => (
                    StatusCodes.Status400BadRequest,
                    ApiResponse<object>.Failure(
                        valEx.Errors.SelectMany(kvp => kvp.Value.Select(err => $"{kvp.Key}: {err}")).ToList(),
                        "Validation failed."
                    )
                ),

                NotFoundException notFoundEx => (
                    StatusCodes.Status404NotFound,
                    ApiResponse<object>.Failure(notFoundEx.Message, "Resource not found.")
                ),

                UnauthorizedAccessException unauthEx => (
                    StatusCodes.Status401Unauthorized,
                    ApiResponse<object>.Failure(unauthEx.Message, "Unauthorized access.")
                ),

                ForbiddenAccessException forbiddenEx => (
                    StatusCodes.Status403Forbidden,
                    ApiResponse<object>.Failure(forbiddenEx.Message, "Access forbidden.")
                ),

                DbUpdateConcurrencyException => (
                    StatusCodes.Status409Conflict,
                    ApiResponse<object>.Failure("The record was modified or deleted by another operation.", "Concurrency conflict.")
                ),

                DbUpdateException dbEx => (
                    StatusCodes.Status409Conflict,
                    ApiResponse<object>.Failure(
                        isDev ? (dbEx.InnerException?.Message ?? dbEx.Message) : "A database constraint violation occurred.",
                        "Database Error"
                    )
                ),

                InvalidOperationException invOpEx => (
                    StatusCodes.Status400BadRequest,
                    ApiResponse<object>.Failure(invOpEx.Message, "Business rule violation.")
                ),

                ArgumentException argEx => (
                    StatusCodes.Status400BadRequest,
                    ApiResponse<object>.Failure(argEx.Message, "Invalid argument.")
                ),

                _ => (
                    StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Failure(
                        isDev
                            ? new List<string> { actualException.Message, actualException.StackTrace?.Split('\n').FirstOrDefault()?.Trim() ?? string.Empty }
                            : new List<string> { "An unexpected error occurred. Please try again later." },
                        "Internal Server Error"
                    )
                )
            };
        }
    }
}