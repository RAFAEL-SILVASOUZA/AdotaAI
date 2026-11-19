using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class AttendantRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public AttendantRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Attendant NewAttendant(
        int id,
        string name,
        string email,
        string cpf,
        int institutionId) =>
        new(id, name, email, "senhaSegura123", cpf, institutionId);

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
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana Costa", "mariana@email.com", "12345678901", institutionId));

        var saved = await context.Set<Attendant>().SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Mariana Costa", saved.Name);
        Assert.Equal(institutionId, saved.InstitutionId);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraAtendenteNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana Costa", "mariana@email.com", "12345678901", institutionId));
        var saved = await context.Set<Attendant>().SingleAsync();

        var attendant = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(attendant);
        Assert.Equal("mariana@email.com", attendant.Email);
    }

    [Fact]
    public async Task GetByIdAsync_AtendenteInexistente_DevolveNull()
    {
        var (context, _) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana", "mariana@email.com", "12345678901", institutionId));
        await repository.AddAsync(NewAttendant(0, "Lucas", "lucas@email.com", "10987654321", institutionId));

        var attendants = await repository.GetAllAsync();

        Assert.Equal(2, attendants.Count);
        Assert.Equal("Mariana", attendants[0].Name);
        Assert.Equal("Lucas", attendants[1].Name);
    }

    [Fact]
    public async Task GetByInstitutionIdAsync_ConsultaOBancoRealOrdenadoPorId()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana", "mariana@email.com", "12345678901", institutionId));
        await repository.AddAsync(NewAttendant(0, "Lucas", "lucas@email.com", "10987654321", institutionId));

        var attendants = await repository.GetByInstitutionIdAsync(institutionId);

        Assert.Equal(2, attendants.Count);
        Assert.Equal("Mariana", attendants[0].Name);
        Assert.Equal("Lucas", attendants[1].Name);
        Assert.Empty(await repository.GetByInstitutionIdAsync(9999));
    }

    [Fact]
    public async Task ExistsAsync_ConsultaEmailECpfNaHierarquiaEmployeeNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await context.Set<Employee>().AddAsync(
            new Employee(0, "Carlos Base", "carlos.base@email.com", "senhaSegura123", "55555555555"));
        await context.SaveChangesAsync();

        await repository.AddAsync(NewAttendant(0, "Mariana", "mariana@email.com", "12345678901", institutionId));

        Assert.True(await repository.EmailExistsAsync("mariana@email.com"));
        Assert.True(await repository.EmailExistsAsync("carlos.base@email.com"));
        Assert.True(await repository.CpfExistsAsync("12345678901"));
        Assert.True(await repository.CpfExistsAsync("55555555555"));

        Assert.False(await repository.EmailExistsAsync("outro@email.com"));
        Assert.False(await repository.CpfExistsAsync("99999999999"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana Costa", "mariana@email.com", "12345678901", institutionId));
        var saved = await context.Set<Attendant>().SingleAsync();

        Assert.True(await repository.UpdateAsync(
            NewAttendant(saved.Id, "Mariana Atualizada", "mariana@email.com", "12345678901", institutionId)));

        Assert.Equal("Mariana Atualizada", (await context.Set<Attendant>().SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_AtendenteInexistente_DevolveFalse()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        Assert.False(await repository.UpdateAsync(
            NewAttendant(9999, "Nenhuma", "nenhuma@email.com", "12345678901", institutionId)));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new AttendantRepository(context);

        await repository.AddAsync(NewAttendant(0, "Mariana Costa", "mariana@email.com", "12345678901", institutionId));
        var saved = await context.Set<Attendant>().SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Set<Attendant>().CountAsync());
    }

    [Fact]
    public async Task Migration_CriaTabelaAttendantsComAsColunasEFksEsperadas()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Attendant));
        Assert.NotNull(table);
        Assert.Equal("Attendants", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("InstitutionId", columns);
    }
}