using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class PetServiceTests
{
    private readonly FakePetRepository _repository = new();
    private readonly FakeInstitutionRepository _institutionRepository = new();
    private readonly IValidator<Pet> _validator = new PetValidator();
    private readonly PetService _service;

    public PetServiceTests()
    {
        _institutionRepository.AddAsync(new Institution(
            0, "Abrigo Amigo", "abrigo@email.com", "44999999999", "12345678987654",
            "foto.png", "senhaSegura123", "Abrigo dedicado ao resgate de animais.",
            "08:00 às 18:00", "Rua das Flores, 123")).GetAwaiter().GetResult();

        _service = new PetService(_repository, _institutionRepository, _validator);
    }

    private static CreatePetDto CreateDto(
        string name = "Rex",
        Species species = Species.Dog,
        string race = "Vira-lata",
        string photo = "rex.jpg",
        int age = 3,
        bool isFemale = false,
        Size petSize = Size.Medium,
        string behaviourDesc = "Dócil e brincalhão.",
        string vetRecord = "Vacinas em dia.",
        int institutionId = 1,
        Status petStatus = Status.Available) =>
        new(name, species, race, photo, age, isFemale, petSize, behaviourDesc, vetRecord, institutionId, petStatus);

    [Fact]
    public async Task CreateAsync_ComDadosValidos_SalvaPetEUsaIdGeradoPeloBanco()
    {
        var id = await _service.CreateAsync(CreateDto());

        Assert.Equal(1, id);
        Assert.Single(_repository.Pets);
        Assert.Equal(1, _repository.Pets.First().Id);
        Assert.Equal(1, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ComCamposObrigatoriosInvalidos_LancaValidationFailedExceptionComMensagensCertas()
    {
        var dto = CreateDto(
            name: "",
            species: (Species)99,
            race: "",
            photo: "",
            age: -1,
            petSize: (Size)99,
            behaviourDesc: "",
            vetRecord: "",
            institutionId: 0,
            petStatus: (Status)99);

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() => _service.CreateAsync(dto));

        Assert.DoesNotContain("O id do pet deve ser maior que zero.", ex.Errors);
        Assert.Contains("O nome do pet não pode ser vazio.", ex.Errors);
        Assert.Contains("A raça do pet não pode ser vazia.", ex.Errors);
        Assert.Contains("A idade do pet deve estar entre 0 e 30 anos.", ex.Errors);
        Assert.Contains("A foto do pet não pode ser vazia.", ex.Errors);
        Assert.Contains("O prontuário veterinário não pode ser vazio.", ex.Errors);
        Assert.Contains("A descrição de comportamento não pode ser vazia.", ex.Errors);
        Assert.Contains("O id da instituição deve ser maior que zero.", ex.Errors);
        Assert.Equal(0, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task GetByIdAsync_ComPetExistente_DevolveDto()
    {
        await _repository.AddAsync(PetFactory.Valid());

        var dto = await _service.GetByIdAsync(1);

        Assert.Equal(1, dto.Id);
        Assert.Equal("Rex", dto.Name);
        Assert.Null(typeof(PetResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task GetByIdAsync_ComPetInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));

        Assert.Equal("Pet com id 99 não encontrado.", ex.Message);
    }

    [Fact]
    public async Task GetByInstitutionIdAsync_FiltraPetsDaInstituicao()
    {
        await _repository.AddAsync(PetFactory.Valid(institutionId: 10));
        await _repository.AddAsync(PetFactory.Valid(institutionId: 20));

        var dtos = await _service.GetByInstitutionIdAsync(10);

        Assert.Single(dtos);
        Assert.Equal(10, dtos[0].InstitutionId);
    }

    [Fact]
    public async Task UpdateAsync_ComPetExistente_PersisteAlteracoes()
    {
        await _repository.AddAsync(PetFactory.Valid());

        var dto = new UpdatePetDto(
            "Rex Atualizado", Species.Dog, "Labrador", 4, false,
            Size.Large, "rex2.jpg", "Castrado.", "Calmo.", Status.InProcess);

        Assert.True(await _service.UpdateAsync(1, dto));

        var saved = await _service.GetByIdAsync(1);
        Assert.Equal("Rex Atualizado", saved.Name);
        Assert.Equal(Status.InProcess, saved.PetStatus);
    }

    [Fact]
    public async Task UpdateAsync_ComPetInexistente_LancaNotFoundException()
    {
        var dto = new UpdatePetDto(
            "Rex", Species.Dog, "Vira-lata", 3, false,
            Size.Medium, "rex.jpg", "Vacinas em dia.", "Dócil.", Status.Available);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(99, dto));

        Assert.Equal("Pet com id 99 não encontrado.", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_ComPetExistente_RemovePet()
    {
        await _repository.AddAsync(PetFactory.Valid());

        Assert.True(await _service.DeleteAsync(1));
        Assert.Empty(_repository.Pets);
    }

    [Fact]
    public async Task DeleteAsync_ComPetInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));

        Assert.Equal("Pet com id 99 não encontrado.", ex.Message);
    }
}