using ClienteService.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace ClienteService.Application.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (AppException ex)
            {
                _logger.LogError(ex, "Un error de aplicación ha ocurrido: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex.Message, ex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Un error inesperado ha ocurrido.");
                await HandleExceptionAsync(context, "Ha ocurrido un error inesperado. Intente nuevamente más tarde.",
                (int)HttpStatusCode.InternalServerError);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(response);
        }
    }

}
