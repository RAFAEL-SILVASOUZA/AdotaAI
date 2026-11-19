using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class PetValidatorTests
{
    private readonly PetValidator _validator = new();

    private ValidationResult Validate(Pet pet) => _validator.Validate(pet);

    [Fact]
    public void PetValido_PassaEmTodasAsRegras()
    {
        var result = Validate(PetFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var pet = With(PetFactory.Valid(), id: 0);

        var result = Validate(pet);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(PetFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Name));
    }

    [Fact]
    public void Nome_MaiorQue50Caracteres_EInvalido()
    {
        var result = Validate(With(PetFactory.Valid(), name: new string('a', 51)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Raca_Invalida(string race)
    {
        var result = Validate(With(PetFactory.Valid(), race: race));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Race));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(31)]
    public void Idade_Invalida(int age)
    {
        var result = Validate(With(PetFactory.Valid(), age: age));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Age));
    }

    [Fact]
    public void Foto_Vazia_EInvalida()
    {
        var result = Validate(With(PetFactory.Valid(), photo: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Photo));
    }

    [Fact]
    public void ProntuarioVeterinario_Vazio_EInvalido()
    {
        var result = Validate(With(PetFactory.Valid(), vetRecord: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.VetRecord));
    }

    [Fact]
    public void DescricaoComportamento_Vazia_EInvalida()
    {
        var result = Validate(With(PetFactory.Valid(), behaviourDesc: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.BehaviourDesc));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InstitutionId_MenorOuIgualAZero_EInvalido(int institutionId)
    {
        var result = Validate(With(PetFactory.Valid(), institutionId: institutionId));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.InstitutionId));
    }

    [Theory]
    [InlineData((Species)0)]
    [InlineData((Species)99)]
    public void Especie_ForaDoEnum_EInvalida(Species species)
    {
        var result = Validate(With(PetFactory.Valid(), species: species));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.Species));
    }

    [Theory]
    [InlineData((Size)0)]
    [InlineData((Size)99)]
    public void Porte_ForaDoEnum_EInvalido(Size petSize)
    {
        var result = Validate(With(PetFactory.Valid(), petSize: petSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.PetSize));
    }

    [Theory]
    [InlineData((Status)0)]
    [InlineData((Status)99)]
    public void Status_ForaDoEnum_EInvalido(Status petStatus)
    {
        var result = Validate(With(PetFactory.Valid(), petStatus: petStatus));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Pet.PetStatus));
    }

    private static Pet With(
        Pet pet,
        int? id = null,
        string? name = null,
        Species? species = null,
        string? race = null,
        int? age = null,
        bool? isFemale = null,
        Size? petSize = null,
        string? photo = null,
        string? vetRecord = null,
        string? behaviourDesc = null,
        Status? petStatus = null,
        int? institutionId = null) =>
        new(
            id: id ?? pet.Id,
            name: name ?? pet.Name,
            species: species ?? pet.Species,
            race: race ?? pet.Race,
            age: age ?? pet.Age,
            isFemale: isFemale ?? pet.IsFemale,
            petSize: petSize ?? pet.PetSize,
            photo: photo ?? pet.Photo,
            vetRecord: vetRecord ?? pet.VetRecord,
            behaviourDesc: behaviourDesc ?? pet.BehaviourDesc,
            petStatus: petStatus ?? pet.PetStatus,
            institutionId: institutionId ?? pet.InstitutionId);
}