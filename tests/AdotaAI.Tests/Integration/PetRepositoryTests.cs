using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using AdotaAI.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class PetRepositoryTests : IClassFixture<SqliteDatabaseFixture>
{
    private readonly SqliteDatabaseFixture _db;

    public PetRepositoryTests(SqliteDatabaseFixture db)
    {
        _db = db;
    }

    private static Pet NewPet(int id, string name, int institutionId) =>
        new(
            id: id,
            name: name,
            species: Species.Dog,
            race: "Vira-lata",
            age: 3,
            isFemale: false,
            petSize: Size.Medium,
            photo: "rex.jpg",
            vetRecord: "Vacinas em dia.",
            behaviourDesc: "Dócil e brincalhão.",
            petStatus: Status.Available,
            institutionId: institutionId);

    private (AdotaAIDbContext Context, int InstitutionId) FreshContextWithInstitution()
    {
        var context = _db.CreateContext();
        context.Pets.ExecuteDeleteAsync().GetAwaiter().GetResult();
        context.Institutions.ExecuteDeleteAsync().GetAwaiter().GetResult();

        var institution = new Institution(
            0, "Abrigo Amigo", "abrigo@email.com", "44999999999", "12345678987654",
            "foto.png", "senhaSegura123", "Abrigo dedicado ao resgate de animais.",
            "08:00 às 18:00", "Rua das Flores, 123");

        context.Institutions.Add(institution);
        context.SaveChanges();

        return (context, institution.Id);
    }

    [Fact]
    public async Task AddAsync_PersisteNoBancoRealEIdEGeradoPeloSqlite()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));

        var saved = await context.Pets.SingleAsync();
        Assert.True(saved.Id > 0);
        Assert.Equal("Rex", saved.Name);
    }

    [Fact]
    public async Task GetByIdAsync_EncontraPetNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));
        var saved = await context.Pets.SingleAsync();

        var pet = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(pet);
        Assert.Equal("Rex", pet.Name);
    }

    [Fact]
    public async Task GetByIdAsync_PetInexistente_DevolveNull()
    {
        var (context, _) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        Assert.Null(await repository.GetByIdAsync(9999));
    }

    [Fact]
    public async Task GetAllAsync_DevolveTodosOrdenadoPorId()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));
        await repository.AddAsync(NewPet(0, "Thor", institutionId));

        var pets = await repository.GetAllAsync();

        Assert.Equal(2, pets.Count);
        Assert.Equal("Rex", pets[0].Name);
        Assert.Equal("Thor", pets[1].Name);
    }

    [Fact]
    public async Task GetByInstitutionIdAsync_ConsultaOBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));

        var pets = await repository.GetByInstitutionIdAsync(institutionId);
        Assert.Single(pets);
        Assert.Empty(await repository.GetByInstitutionIdAsync(9999));
    }

    [Fact]
    public async Task UpdateAsync_PersisteAlteracaoNoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));
        var saved = await context.Pets.SingleAsync();

        Assert.True(await repository.UpdateAsync(NewPet(saved.Id, "Rex Atualizado", institutionId)));
        Assert.Equal("Rex Atualizado", (await context.Pets.SingleAsync()).Name);
    }

    [Fact]
    public async Task UpdateAsync_PetInexistente_DevolveFalse()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        Assert.False(await repository.UpdateAsync(NewPet(9999, "Nenhum", institutionId)));
    }

    [Fact]
    public async Task DeleteAsync_RemoveDoBancoReal()
    {
        var (context, institutionId) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        await repository.AddAsync(NewPet(0, "Rex", institutionId));
        var saved = await context.Pets.SingleAsync();

        Assert.True(await repository.DeleteAsync(saved.Id));
        Assert.Equal(0, await context.Pets.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_PetInexistente_DevolveFalse()
    {
        var (context, _) = FreshContextWithInstitution();
        using var _ = context;
        var repository = new PetRepository(context);

        Assert.False(await repository.DeleteAsync(9999));
    }

    [Fact]
    public async Task Migration_CriaTabelaPetsComAsColunasEsperadas()
    {
        using var context = _db.CreateContext();

        var table = context.Model.FindEntityType(typeof(Pet));
        Assert.NotNull(table);
        Assert.Equal("Pets", table.GetTableName());

        var columns = table.GetProperties().Select(p => p.GetColumnName()).ToList();
        Assert.Contains("Id", columns);
        Assert.Contains("Name", columns);
        Assert.Contains("Species", columns);
        Assert.Contains("Race", columns);
        Assert.Contains("Age", columns);
        Assert.Contains("IsFemale", columns);
        Assert.Contains("PetSize", columns);
        Assert.Contains("Photo", columns);
        Assert.Contains("VetRecord", columns);
        Assert.Contains("BehaviourDesc", columns);
        Assert.Contains("PetStatus", columns);
        Assert.Contains("InstitutionId", columns);
    }
}