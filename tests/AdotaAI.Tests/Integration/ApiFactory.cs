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

    public async Task ClearInstitutionsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();
        await context.Set<Domain.Institution>().ExecuteDeleteAsync();
    }

    public async Task ClearPetsAndInstitutionsAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();
        await context.Set<Domain.Pet>().ExecuteDeleteAsync();
        await context.Set<Domain.Institution>().ExecuteDeleteAsync();
    }

    // Limpa todas as tabelas na ordem correta das FKs e herança TPT (filhos -> pais).
    public async Task ResetDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();

        await context.Database.ExecuteSqlRawAsync("DELETE FROM Pets;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Veterinarians;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Attendants;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Adms;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Employees;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Institutions;");
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Users;");
    }

    // Insere uma Institution válida direto no SQLite temporário,
    // contornando o bug atual do POST /api/Institution para testar Pet e Employees.
    public async Task<int> SeedInstitutionAsync(string email = "abrigo@email.com")
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AdotaAIDbContext>();

        var institution = new Domain.Institution(
            id: 0,
            name: "Abrigo Amigo",
            email: email,
            phone: "44999999999",
            document: "12345678987654",
            photo: "foto.png",
            password: "senhaSegura123",
            description: "Abrigo dedicado ao resgate de animais.",
            operatingHours: "08:00 às 18:00",
            address: "Rua das Flores, 123");

        await context.Set<Domain.Institution>().AddAsync(institution);
        await context.SaveChangesAsync();
        return institution.Id;
    }

    public void Dispose()
    {
        _factory.Dispose();

        if (File.Exists(DbPath))
            File.Delete(DbPath);
    }
}