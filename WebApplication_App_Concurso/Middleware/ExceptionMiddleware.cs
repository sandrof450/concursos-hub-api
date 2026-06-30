using System.Net;
using System.Text.Json;
using WebApplication_App_Concurso.Exceptions;

namespace WebApplication_App_Concurso.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
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
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new
                {
                    erro = "Erro interno inesperado.",
                    detalhesErro = ex,
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
