// Integration/Setup/CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication_App_Concurso.Models;
using WebApplication_App_Concurso.Repositories.Contexts;

namespace WebApplication_App_Concurso.Tests.Integration.Setup;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // remove o DbContext real
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<DataContext>) ||
                    d.ServiceType == typeof(DataContext) ||
                    d.ServiceType.Name.Contains("DbContext"))
                .ToList();

            foreach (var descriptor in descriptors)
                services.Remove(descriptor);

            // adiciona InMemory com nome fixo
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase("TestDb"));
                       //.EnableServiceProviderCaching(false));

            // seed feito aqui — depois que todos os serviços foram registrados
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Database.EnsureCreated();
            SeedData(context);
        });
    }

    private void SeedData(DataContext context)
    {
        if (context.Concursos.Any()) return;

        context.Concursos.AddRange(
            new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Concurso AL", Estado = "AL", Vagas = 10, Fonte = "PCIConcursos", DataPublicacao = DateTime.UtcNow },
            new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Concurso SP", Estado = "SP", Vagas = 20, Fonte = "ConcursosBrasil", DataPublicacao = DateTime.UtcNow },
            new Concurso { ConcursoId = Guid.NewGuid(), Titulo = "Concurso MG", Estado = "MG", Vagas = 5, Fonte = "PCIConcursos", DataPublicacao = DateTime.UtcNow }
        );
        context.SaveChanges();
    }
}