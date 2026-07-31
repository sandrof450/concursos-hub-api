using System.Net;
using System.Text.Json;
using WebApplication_App_Concurso.Exceptions;

namespace WebApplication_App_Concurso.Middleware
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
            catch (AppException ex)
            {
                _logger.LogWarning(ex.InnerException ?? ex,
                    "AppException tratada: {Message} | Inner: {InnerMessage}",
                    ex.Message, ex.InnerException?.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.StatusCode;
                var result = JsonSerializer.Serialize(new
                {
                    messageError = ex.Message
                });
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno inesperado.");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var result = JsonSerializer.Serialize(new
                {
                    erro = "Erro interno inesperado.",
                    detalhesErro = ex.Message,
                });
                await context.Response.WriteAsync(result);
            }
        }
    }
}