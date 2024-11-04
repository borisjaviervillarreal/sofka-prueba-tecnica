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
            _logger.LogError(context.Exception, "A ocurrido un error");

            var statusCode = context.Exception switch
            {
                ClienteNotFoundException => StatusCodes.Status404NotFound,
                AppException appEx => appEx.StatusCode,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Un error ha ocurrido durante el procesamiento de su petición.",
                Detail = context.Exception.Message
            };

            context.Result = new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
        }
    }
}
