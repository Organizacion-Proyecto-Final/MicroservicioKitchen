using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        
        private readonly JsonSerializerOptions _jsonOptions;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;

            // Inicializar JsonOptions en el constructor
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = environment.IsDevelopment()
            };
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
            var correlationId = Guid.NewGuid().ToString();
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            var (statusCode, problemDetails) = MapExceptionToProblemDetails(context, exception, correlationId);

            LogException(exception, correlationId, statusCode);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var json = JsonSerializer.Serialize(problemDetails, _jsonOptions);
            await context.Response.WriteAsync(json);
        }

        private (HttpStatusCode statusCode, ProblemDetails problemDetails) MapExceptionToProblemDetails(
            HttpContext context,
            Exception exception,
            string correlationId)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var title = "An unexpected error occurred";
            var detail = "An internal server error occurred. Please try again later.";
            var errorCode = "INTERNAL_ERROR";
            object? errors = null;

            switch (exception)
            {
                case NotFoundException notFound:
                    statusCode = HttpStatusCode.NotFound;
                    title = "Resource not found";
                    detail = notFound.Message;
                    errorCode = notFound.ErrorCode;
                    break;

                case ValidationExceptions validation:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Validation failed";
                    detail = validation.Message;
                    errorCode = validation.ErrorCode;
                    errors = validation.Errors;
                    break;

                case ConflictException conflict:
                    statusCode = HttpStatusCode.Conflict;
                    title = "Resource conflict";
                    detail = conflict.Message;
                    errorCode = conflict.ErrorCode;
                    break;

                case ConcurrencyException concurrency:
                    statusCode = HttpStatusCode.Conflict;
                    title = "Concurrency conflict";
                    detail = concurrency.Message;
                    errorCode = concurrency.ErrorCode;
                    break;

                case DomainException domain:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Business rule violation";
                    detail = domain.Message;
                    errorCode = domain.ErrorCode;
                    break;

                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Invalid argument";
                    detail = argEx.Message;
                    errorCode = "INVALID_ARGUMENT";
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    title = "Unauthorized";
                    detail = "You are not authorized to perform this action.";
                    errorCode = "UNAUTHORIZED";
                    break;

                case InvalidOperationException invalidOp:
                    statusCode = HttpStatusCode.BadRequest;
                    title = "Invalid operation";
                    detail = invalidOp.Message;
                    errorCode = "INVALID_OPERATION";
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    title = "Resource not found";
                    detail = exception.Message;
                    errorCode = "NOT_FOUND";
                    break;
            }

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
                Type = $"https://httpstatuses.com/{(int)statusCode}",
                Extensions =
            {
                ["errorCode"] = errorCode,
                ["correlationId"] = correlationId,
                ["timestamp"] = DateTime.UtcNow
            }
            };

            if (errors != null)
            {
                problemDetails.Extensions["errors"] = errors;
            }

            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
                problemDetails.Extensions["source"] = exception.Source;
                if (exception.InnerException != null)
                {
                    problemDetails.Extensions["innerException"] = exception.InnerException.Message;
                }
            }

            return (statusCode, problemDetails);
        }

        private void LogException(Exception exception, string correlationId, HttpStatusCode statusCode)
        {
            var logData = new
            {
                CorrelationId = correlationId,
                StatusCode = (int)statusCode,
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message
            };

            switch (statusCode)
            {
                case HttpStatusCode.InternalServerError:
                    _logger.LogError(exception, "Error interno: {CorrelationId} - {ExceptionType}: {Message}",
                        correlationId, exception.GetType().Name, exception.Message);
                    break;

                case HttpStatusCode.NotFound:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Conflict:
                    _logger.LogWarning("Error de negocio: {CorrelationId} - {ExceptionType}: {Message}",
                        correlationId, exception.GetType().Name, exception.Message);
                    break;

                default:
                    _logger.LogError(exception, "Error inesperado: {CorrelationId} - {ExceptionType}: {Message}",
                        correlationId, exception.GetType().Name, exception.Message);
                    break;
            }
        }
    }

}
