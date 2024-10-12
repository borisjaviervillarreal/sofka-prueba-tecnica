using CuentaService.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace CuentaService.Application.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                await HandleExceptionAsync(context, ex.Message, ex.StatusCode);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, "Ha ocurrido un error inesperado. Intente nuevamente más tarde.", (int)HttpStatusCode.InternalServerError);
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
