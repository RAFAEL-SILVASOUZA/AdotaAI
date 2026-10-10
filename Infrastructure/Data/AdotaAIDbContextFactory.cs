using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdotaAI.Infrastructure.Data;

// Usada apenas pelo dotnet ef (design-time), para gerar migrations
// sem precisar executar o Program.cs.
public class AdotaAIDbContextFactory : IDesignTimeDbContextFactory<AdotaAIDbContext>
{
    public AdotaAIDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AdotaAIDbContext>()
            .UseSqlite("Data Source=adotaai.db")
            .Options;

        return new AdotaAIDbContext(options);
    }
}
