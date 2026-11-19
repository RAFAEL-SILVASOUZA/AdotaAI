using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class AdmRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public AdmRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Adm NewAdm(int id, string name, string email, string cpf) =>
        new(id, name, email, "senhaSegura123", cpf);

    private AdotaAIDbContext FreshContext()
    {
        var context = _db.CreateContext();
        context.Database.ExecuteSqlRaw("DELETE FROM Veterinarians;");
        context.Database.ExecuteSqlRaw("DELETE FROM Attendants;");
        context.Database.ExecuteSqlRaw("DELETE FROM Adms;");
        context.Database.ExecuteSqlRaw("DELETE FROM Employees;");
        return context;
    }

    [Fact]
    public async Task AddAsync_PersisteNoBancoRealEIdEGeradoPeloSqlite()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await repository.AddAsync(NewAdm(0, "Fernanda Lima", "fernanda@email.com", "12345678901"));

        var saved = await context.Set<Adm>().SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Fernanda Lima", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraAdmNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await repository.AddAsync(NewAdm(0, "Fernanda Lima", "fernanda@email.com", "12345678901"));
        var saved = await context.Set<Adm>().SingleAsync();

        var adm = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(adm);
        Assert.Equal("fernanda@email.com", adm.Email);
    }

    [Fact]
    public async Task GetByIdAsync_AdmInexistente_DevolveNull()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await repository.AddAsync(NewAdm(0, "Fernanda", "fernanda@email.com", "12345678901"));
        await repository.AddAsync(NewAdm(0, "Helena", "helena@email.com", "10987654321"));

        var adms = await repository.GetAllAsync();

        Assert.Equal(2, adms.Count);
        Assert.Equal("Fernanda", adms[0].Name);
        Assert.Equal("Helena", adms[1].Name);
    }

    [Fact]
    public async Task ExistsAsync_ConsultaEmailECpfNaHierarquiaEmployeeNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await context.Set<Employee>().AddAsync(
            new Employee(0, "Carlos Base", "carlos.base@email.com", "senhaSegura123", "55555555555"));
        await context.SaveChangesAsync();

        await repository.AddAsync(NewAdm(0, "Fernanda", "fernanda@email.com", "12345678901"));

        Assert.True(await repository.EmailExistsAsync("fernanda@email.com"));
        Assert.True(await repository.EmailExistsAsync("carlos.base@email.com"));
        Assert.True(await repository.CpfExistsAsync("12345678901"));
        Assert.True(await repository.CpfExistsAsync("55555555555"));

        Assert.False(await repository.EmailExistsAsync("outro@email.com"));
        Assert.False(await repository.CpfExistsAsync("99999999999"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await repository.AddAsync(NewAdm(0, "Fernanda Lima", "fernanda@email.com", "12345678901"));
        var saved = await context.Set<Adm>().SingleAsync();

        Assert.True(await repository.UpdateAsync(
            NewAdm(saved.Id, "Fernanda Atualizada", "fernanda@email.com", "12345678901")));

        Assert.Equal("Fernanda Atualizada", (await context.Set<Adm>().SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_AdmInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        Assert.False(await repository.UpdateAsync(
            NewAdm(9999, "Nenhuma", "nenhuma@email.com", "12345678901")));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        using var context = FreshContext();
        var repository = new AdmRepository(context);

        await repository.AddAsync(NewAdm(0, "Fernanda Lima", "fernanda@email.com", "12345678901"));
        var saved = await context.Set<Adm>().SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Set<Adm>().CountAsync());
    }

    [Fact]
    public async Task Migration_CriaTabelaAdmsComAColunaEFkEsperadas()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Adm));
        Assert.NotNull(table);
        Assert.Equal("Adms", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
    }
}