using Microsoft.AspNetCore.Http.Features;
using WebApplication_App_Concurso.Services.Interfaces;

namespace WebApplication_App_Concurso.Jobs
{
    public class ConcursosJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        
        public ConcursosJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task ExecuteAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var concursoService = scope.ServiceProvider.GetRequiredService<IConcursoService>();
            await concursoService.CreateConcursosAsync();
        }

    }
}
