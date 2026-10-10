using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Tests.Integration;

// Banco SQLite real em arquivo temporário, criado pelas migrations
// (mesmo caminho que o app usa em produção).
public class SqliteDatabaseFixture : IDisposable
{
    public string DbPath { get; } = Path.Combine(
        Path.GetTempPath(), $"adotaai_test_{Guid.NewGuid()}.db");

    public AdotaAIDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AdotaAIDbContext>()
            // Pool=False: sem pooling o arquivo é liberado no Dispose do contexto,
            // o que permite apagar o banco de teste ao final da classe.
            .UseSqlite($"Data Source={DbPath};Pooling=False")
            .Options;

        var context = new AdotaAIDbContext(options);
        context.Database.Migrate();
        return context;
    }

    public void Dispose()
    {
        if (File.Exists(DbPath))
            File.Delete(DbPath);
    }
}
