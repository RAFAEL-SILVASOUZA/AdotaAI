using AdotaAI.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AdotaAI.Tests.Integration;

// Sobe o app real (WebApplicationFactory) com SQLite em arquivo temporário,
// para testar os endpoints sem usar o adotaai.db do projeto.
public class ApiFactory : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public string DbPath { get; } = Path.Combine(
        Path.GetTempPath(), $"adotaai_api_test_{Guid.NewGuid()}.db");

    public HttpClient Client { get; }

    public ApiFactory()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                // Production garante que o middleware de erro do Program.cs responde
                // (em Development a developer exception page devolveria 500).
                builder.UseEnvironment("Production");

                builder.ConfigureServices(services =>
                {
                    var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AdotaAIDbContext>));
                    services.Remove(descriptor);

                    services.AddDbContext<AdotaAIDbContext>(options =>
                        options.UseSqlite($"Data Source={DbPath};Pooling=False"));
                });
            });

        Client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();
        context.Database.Migrate();
    }

    // Cada teste começa com a tabela limpa, para não depender da ordem de execução.
    public async Task ClearUsersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();
        await context.Users.ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        _factory.Dispose();

        if (File.Exists(DbPath))
            File.Delete(DbPath);
    }
}
