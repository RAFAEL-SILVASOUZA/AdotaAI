using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class EmployeeRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public EmployeeRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Employee NewEmployee(int id, string name, string email, string cpf) =>
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
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos Souza", "carlos@email.com", "12345678901"));

        var saved = await context.Set<Employee>().SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Carlos Souza", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraFuncionarioNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos Souza", "carlos@email.com", "12345678901"));
        var saved = await context.Set<Employee>().SingleAsync();

        var employee = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(employee);
        Assert.Equal("carlos@email.com", employee.Email);
    }

    [Fact]
    public async Task GetByIdAsync_FuncionarioInexistente_DevolveNull()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos", "carlos@email.com", "12345678901"));
        await repository.AddAsync(NewEmployee(0, "Bruno", "bruno@email.com", "10987654321"));

        var employees = await repository.GetAllAsync();

        Assert.Equal(2, employees.Count);
        Assert.Equal("Carlos", employees[0].Name);
        Assert.Equal("Bruno", employees[1].Name);
    }

    [Fact]
    public async Task ExistsAsync_ConsultaEmailECpfNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos Souza", "carlos@email.com", "12345678901"));

        Assert.True(await repository.EmailExistsAsync("carlos@email.com"));
        Assert.True(await repository.CpfExistsAsync("12345678901"));
        Assert.False(await repository.EmailExistsAsync("outro@email.com"));
        Assert.False(await repository.CpfExistsAsync("99999999999"));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos Souza", "carlos@email.com", "12345678901"));
        var saved = await context.Set<Employee>().SingleAsync();

        Assert.True(await repository.UpdateAsync(
            NewEmployee(saved.Id, "Carlos Atualizado", "carlos@email.com", "12345678901")));

        Assert.Equal("Carlos Atualizado", (await context.Set<Employee>().SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_FuncionarioInexistente_DevolveFalse()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        Assert.False(await repository.UpdateAsync(
            NewEmployee(9999, "Nenhum", "nenhum@email.com", "12345678901")));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos Souza", "carlos@email.com", "12345678901"));
        var saved = await context.Set<Employee>().SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Set<Employee>().CountAsync());
    }

    [Fact]
    public async Task EmailDuplicado_IndiceUnicoDoBancoLancaDbUpdateException()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos", "carlos@email.com", "12345678901"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(NewEmployee(0, "Bruno", "carlos@email.com", "10987654321")));

        Assert.Contains("UNIQUE", ex.InnerException?.Message ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CpfDuplicado_IndiceUnicoDoBancoLancaDbUpdateException()
    {
        using var context = FreshContext();
        var repository = new EmployeeRepository(context);

        await repository.AddAsync(NewEmployee(0, "Carlos", "carlos@email.com", "12345678901"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => repository.AddAsync(NewEmployee(0, "Bruno", "bruno@email.com", "12345678901")));

        Assert.Contains("UNIQUE", ex.InnerException?.Message ?? ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Migration_CriaTabelaEmployeesComAsColunasEIndicesEsperados()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Employee));
        Assert.NotNull(table);
        Assert.Equal("Employees", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("Name", columns);
        Assert.Contains("Email", columns);
        Assert.Contains("Password", columns);
        Assert.Contains("Cpf", columns);

        Assert.Equal(2, table.GetIndexes().Count(i => i.IsUnique));
    }
}