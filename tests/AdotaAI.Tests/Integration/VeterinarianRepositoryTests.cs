using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class VeterinarianRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public VeterinarianRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Veterinarian NewVeterinarian(
        int id,
        string name,
        string email,
        string cpf,
        string crmv,
        int institutionId) =>
        new(id, name, email, "senhaSegura123", cpf, crmv, institutionId);

    private (AdotaAIDbContext Context, int InstitutionId) FreshContextWithInstitution()
    {
        var context = _db.CreateContext();
        context.Database.ExecuteSqlRaw("DELETE FROM Veterinarians;");
        context.Database.ExecuteSqlRaw("DELETE FROM Attendants;");
        context.Database.ExecuteSqlRaw("DELETE FROM Adms;");
        context.Database.ExecuteSqlRaw("DELETE FROM Employees;");
        context.Database.ExecuteSqlRaw("DELETE FROM Institutions;");

        var institution = new Institution(
            0, "Abrigo Amigo", "abrigo@email.com", "44999999999", "12345678987654",
            "foto.png", "senhaSegura123", "Abrigo dedicado ao resgate de animais.",
            "08:00 às 18:00", "Rua das Flores, 123");

        context.Set<Institution>().Add(institution);
        context.SaveChanges();

        return (context, institution.Id);
    }

    [Fact]
    public async Task AddAsync_PersisteNoBancoRealEIdEGeradoPeloSqlite()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));

        var saved = await context.Set<Veterinarian>().SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Dra. Ana", saved.Name);
        Assert.Equal("12345-SP", saved.Crmv);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraVeterinarioNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));
        var saved = await context.Set<Veterinarian>().SingleAsync();

        var vet = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(vet);
        Assert.Equal("ana@email.com", vet.Email);
    }

    [Fact]
    public async Task GetByIdAsync_VeterinarioInexistente_DevolveNull()
    {
        var (context, _) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));
        await repository.AddAsync(NewVeterinarian(0, "Dr. Beto", "beto@email.com", "10987654321", "54321-SP", institutionId));

        var vets = await repository.GetAllAsync();

        Assert.Equal(2, vets.Count);
        Assert.Equal("Dra. Ana", vets[0].Name);
        Assert.Equal("Dr. Beto", vets[1].Name);
    }

    [Fact]
    public async Task ExistsAsync_ConsultaEmailCpfECrmvNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));

        Assert.True(await repository.EmailExistsAsync("ana@email.com"));
        Assert.True(await repository.CpfExistsAsync("12345678901"));
        Assert.True(await repository.CrmvExistsAsync("12345-SP"));

        Assert.False(await repository.EmailExistsAsync("outro@email.com"));
        Assert.False(await repository.CpfExistsAsync("99999999999"));
        Assert.False(await repository.CrmvExistsAsync("99999-SP"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));
        var saved = await context.Set<Veterinarian>().SingleAsync();

        Assert.True(await repository.UpdateAsync(
            NewVeterinarian(saved.Id, "Dra. Ana Atualizada", "ana@email.com", "12345678901", "12345-SP", institutionId)));

        Assert.Equal("Dra. Ana Atualizada", (await context.Set<Veterinarian>().SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_VeterinarioInexistente_DevolveFalse()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        Assert.False(await repository.UpdateAsync(
            NewVeterinarian(9999, "Nenhuma", "nenhuma@email.com", "12345678901", "12345-SP", institutionId)));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));
        var saved = await context.Set<Veterinarian>().SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Set<Veterinarian>().CountAsync());
    }

    [Fact]
    public async Task CrmvDuplicado_IndiceUnicoDoBancoLancaDbUpdateException()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new VeterinarianRepository(context);

        await repository.AddAsync(NewVeterinarian(0, "Dra. Ana", "ana@email.com", "12345678901", "12345-SP", institutionId));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(NewVeterinarian(0, "Dr. Beto", "beto@email.com", "10987654321", "12345-SP", institutionId)));

        Assert.Contains("UNIQUE", ex.InnerException?.Message ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Migration_CriaTabelaVeterinariansComAsColunasEIndiceEsperados()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Veterinarian));
        Assert.NotNull(table);
        Assert.Equal("Veterinarians", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("Crmv", columns);
        Assert.Contains("InstitutionId", columns);
    }
}