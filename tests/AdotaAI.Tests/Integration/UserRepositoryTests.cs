using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

// Testes do repositório contra um SQLite real em arquivo temporário,
// criado pelas migrations (mesmo caminho que o app usa).
public class UserRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public UserRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static User NewUser(int id, string name, string email) =>
        new(id, name, email, "11999998888", 25, "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123");

    // Contexto novo com a tabela Users limpa: os testes não dependem da ordem de execução.
    private AdotaAIDbContext FreshContext()
    {
        var context = _db.CreateContext();
        context.Users.ExecuteDeleteAsync().GetAwaiter().GetResult();
        return context;
    }

    [Fact]
    public async Task AddAsync_PersisteNoBancoRealEIdEGeradoPeloSqlite()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));

        var saved = await context.Users.SingleAsync();
        // AUTOINCREMENT não zera entre testes, então o id é verificado como gerado pelo banco.
        Assert.True(saved.Id > 0);
        Assert.Equal("Maria Silva", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraUsuarioNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));
        var saved = await context.Users.SingleAsync();

        var user = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(user);
        Assert.Equal("maria@email.com", user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_UsuarioInexistente_DevolveNull()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Um", "um@email.com"));
        await repository.AddAsync(NewUser(0, "Dois", "dois@email.com"));

        var users = await repository.GetAllAsync();

        Assert.Equal(2, users.Count);
        Assert.Equal("Um", users[0].Name);
        Assert.Equal("Dois", users[1].Name);
    }

    [Fact]
    public async Task EmailExistsAsync_ConsultaOBancoReal()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));

        Assert.True(await repository.EmailExistsAsync("maria@email.com"));
        Assert.False(await repository.EmailExistsAsync("outra@email.com"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));
        var saved = await context.Users.SingleAsync();

        Assert.True(await repository.UpdateAsync(NewUser(saved.Id, "Maria Silva Sousa", "maria@email.com")));

        Assert.Equal("Maria Silva Sousa", (await context.Users.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_UsuarioInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        Assert.False(await repository.UpdateAsync(NewUser(9999, "Ninguém", "ninguem@email.com")));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));
        var saved = await context.Users.SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Users.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_UsuarioInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        Assert.False(await repository.DeleteAsync(9999));
    }

    [Fact]
    public async Task EmailDuplicado_IndiceUnicoDoBancoLancaDbUpdateException()
    {
        using var context = FreshContext();
        var repository = new UserRepository(context);

        await repository.AddAsync(NewUser(0, "Maria Silva", "maria@email.com"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(NewUser(0, "Outra Pessoa", "maria@email.com")));

        Assert.Contains("UNIQUE", ex.InnerException?.Message ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Migration_CriaTabelaUsersComAsColunasEIndiceEsperados()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(User));
        Assert.NotNull(table);
        Assert.Equal("Users", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("Name", columns);
        Assert.Contains("Email", columns);
        Assert.Contains("Phone", columns);
        Assert.Contains("Age", columns);
        Assert.Contains("Photo", columns);
        Assert.Contains("Password", columns);
        Assert.Contains("Gender", columns);
        Assert.Contains("Address", columns);

        var emailIndex = Assert.Single(table.GetIndexes());
        Assert.True(emailIndex.IsUnique);

        var applied = await context.Database.GetAppliedMigrationsAsync();
        Assert.Contains("InitialCreate", string.Join(',', applied));
    }
}
