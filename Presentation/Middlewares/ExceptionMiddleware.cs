using Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace Presentation.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, "Resource not found");
                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error");
                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest, ex.Errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred");
                await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception,
            HttpStatusCode statusCode, object errors = null)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = exception.Message,
                Errors = errors
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
