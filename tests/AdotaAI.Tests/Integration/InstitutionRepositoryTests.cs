using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class InstitutionRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public InstitutionRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Institution NewInstitution(int id, string name, string email) =>
        new(id, name, email, "44999999999", "12345678987654", "foto.png",
            "senhaSegura123", "Instituição dedicada ao resgate de animais.",
            "08:00 às 18:00", "Rua das Flores, 123");

    private AdotaAIDbContext FreshContext()
    {
        var context = _db.CreateContext();
        context.Institutions.ExecuteDeleteAsync().GetAwaiter().GetResult();
        return context;
    }

    [Fact]
    public async Task AddAsync_PersisteNoBancoRealEIdEGeradoPeloSqlite()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));

        var saved = await context.Institutions.SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Instituição Feliz", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraInstituicaoNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));
        var saved = await context.Institutions.SingleAsync();

        var institution = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(institution);
        Assert.Equal("instituicao@email.com", institution.Email);
    }

    [Fact]
    public async Task GetByIdAsync_InstituicaoInexistente_DevolveNull()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Um", "um@email.com"));
        await repository.AddAsync(NewInstitution(0, "Dois", "dois@email.com"));

        var institutions = await repository.GetAllAsync();

        Assert.Equal(2, institutions.Count);
        Assert.Equal("Um", institutions[0].Name);
        Assert.Equal("Dois", institutions[1].Name);
    }

    [Fact]
    public async Task EmailExistsAsync_ConsultaOBancoReal()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));

        Assert.True(await repository.EmailExistsAsync("instituicao@email.com"));
        Assert.False(await repository.EmailExistsAsync("outra@email.com"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));
        var saved = await context.Institutions.SingleAsync();

        Assert.True(await repository.UpdateAsync(
            NewInstitution(saved.Id, "Instituição Atualizada", "instituicao@email.com")));

        Assert.Equal("Instituição Atualizada", (await context.Institutions.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_InstituicaoInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        Assert.False(await repository.UpdateAsync(
            NewInstitution(9999, "Nenhuma", "nenhuma@email.com")));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));
        var saved = await context.Institutions.SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Institutions.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_InstituicaoInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        Assert.False(await repository.DeleteAsync(9999));
    }

    [Fact]
    public async Task EmailDuplicado_IndiceUnicoDoBancoLancaDbUpdateException()
    {
        using var context = FreshContext();
        var repository = new InstitutionRepository(context);

        await repository.AddAsync(NewInstitution(0, "Instituição Feliz", "instituicao@email.com"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(NewInstitution(0, "Outra Instituição", "instituicao@email.com")));

        Assert.Contains("UNIQUE", ex.InnerException?.Message ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Migration_CriaTabelaInstitutionsComAsColunasEIndiceEsperados()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Institution));
        Assert.NotNull(table);
        Assert.Equal("Institutions", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("Name", columns);
        Assert.Contains("Email", columns);
        Assert.Contains("Phone", columns);
        Assert.Contains("Document", columns);
        Assert.Contains("Photo", columns);
        Assert.Contains("Password", columns);
        Assert.Contains("Description", columns);
        Assert.Contains("OperatingHours", columns);
        Assert.Contains("Address", columns);

        Assert.Contains(table.GetIndexes(), i => i.IsUnique);

        var applied = await context.Database.GetAppliedMigrationsAsync();
        Assert.NotEmpty(applied);
    }
}