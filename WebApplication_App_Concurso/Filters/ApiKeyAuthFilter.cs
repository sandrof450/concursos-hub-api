using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication_App_Concurso.Filters
{
    public class ApiKeyAuthFilter: IAsyncActionFilter
    {
        private readonly IConfiguration _configurarion;
        private const string API_KEY_HEADER_NAME = "X-Api-Key";

        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configurarion = configuration;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.HttpContext.Response.StatusCode = 401;
                await context.HttpContext.Response.WriteAsync("API Key was not provided.");
                return;
            }

            var apiKey = _configurarion["Security:CreateConcursoApiKey"];

            if(string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key inválida.");
                return;
            }

            await next();
        }
    }
}
