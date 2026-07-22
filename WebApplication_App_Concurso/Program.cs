using Microsoft.EntityFrameworkCore;
using WebApplication_App_Concurso.Repositories.Contexts;
using WebApplication_App_Concurso.Scrapers;
using WebApplication_App_Concurso.Services;

using WebApplication_App_Concurso.Scrapers.Interfaces;
using WebApplication_App_Concurso.Services.Interfaces;
using WebApplication_App_Concurso.Repositories.Interfaces;
using WebApplication_App_Concurso.Repositories;
using WebApplication_App_Concurso.Middleware;
using Hangfire;
using Hangfire.PostgreSql;
using WebApplication_App_Concurso.Jobs;
using Hangfire.Dashboard;

var builder = WebApplication.CreateBuilder(args);

var isTesting = builder.Environment.IsEnvironment("Testing");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region Swagger configuration
builder.Services.AddSwaggerGen();
#endregion


#region Injeção de dependência
#region HttpClient
builder.Services.AddHttpClient();
#endregion

#region DataContext
builder.Services.AddDbContext<DataContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
#endregion

#region Injection dependency core
builder.Services.AddScoped<IConcursoService, ConcursoService>();
builder.Services.AddScoped<IConcursoRepository, ConcursoRepository>();
builder.Services.AddScoped<IEstadoNormalizadorService, EstadoNormalizadorService>();
builder.Services.AddScoped<IFonteProvider, FonteProvider>();
#endregion

#region Injection jobs
builder.Services.AddScoped<ConcursosJob>();
if (!isTesting)
    builder.Services.AddHangfireServer();
#endregion

#region Scrapers
builder.Services.AddScoped<IScrapingService, ConcursosNoBrasilScraper>();
builder.Services.AddScoped<IScrapingService, PciConcursosScraper>();
#endregion
#endregion

#region Configuração job Hangfire
if (!isTesting)
{
    builder.Services.AddHangfire(config =>
    config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(c =>
            c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
        )
    );
    var url = builder.Configuration["JobsSettings:UrlCreateConcurso"] ?? throw new Exception("UrlCreateConcurso Vazia");
    builder.Services.AddHttpClient("ConcursosApi", client =>
    {
        client.BaseAddress = new Uri(url);
    });
}
#endregion

#region Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:32774/")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
#endregion


builder.Services.AddHealthChecks();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!isTesting)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            IsReadOnlyFunc = (DashboardContext context) => false,
            Authorization = new IDashboardAuthorizationFilter[] { }
        });
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHangfireDashboard("/hangfire");
    }
}

if (!isTesting)
{
    RecurringJob.AddOrUpdate<ConcursosJob>(
        recurringJobId: "SearchConcursos",
        methodCall: job => job.ExecuteAsync(),
        cronExpression: "0 3 * * *"   // todo dia às 03:00
    );
}

app.UseCors("FrontendPolicy");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

if (!isTesting)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DataContext>();
        db.Database.Migrate();
    }
}

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }