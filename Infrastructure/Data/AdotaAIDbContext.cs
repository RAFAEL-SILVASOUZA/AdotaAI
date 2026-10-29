using AdotaAI.Domain;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Infrastructure.Data;

public class AdotaAIDbContext(DbContextOptions<AdotaAIDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Institution> Institutions => Set<Institution>();
    public DbSet<Pet> Pets => Set<Pet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica automaticamente todas as classes IEntityTypeConfiguration<T>
        // da pasta Infrastructure/Data/Configuration.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AdotaAIDbContext).Assembly);
    }
}
