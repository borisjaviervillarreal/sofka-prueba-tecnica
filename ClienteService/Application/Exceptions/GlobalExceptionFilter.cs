using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClienteService.Application.Exceptions
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // Determinamos el código de estado basado en la excepción
            var statusCode = context.Exception switch
            {
                ClienteNotFoundException => StatusCodes.Status404NotFound,
                AppException appEx => appEx.StatusCode,
                _ => StatusCodes.Status500InternalServerError
            };

            // Creamos el objeto ProblemDetails para estructurar la respuesta
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "An error occurred while processing your request.",
                Detail = context.Exception.Message,
                Instance = context.HttpContext.Request.Path
            };

            // Registrar el error
            _logger.LogError(context.Exception, "Error captured by GlobalExceptionFilter");

            // Configuramos el resultado
            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
}
