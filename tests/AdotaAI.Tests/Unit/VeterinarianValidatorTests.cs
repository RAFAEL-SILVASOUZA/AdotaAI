using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class VeterinarianValidatorTests
{
    private readonly VeterinarianValidator _validator = new();

    private ValidationResult Validate(Veterinarian veterinarian) => _validator.Validate(veterinarian);

    [Fact]
    public void VeterinarianValido_PassaEmTodasAsRegras()
    {
        var result = Validate(VeterinarianFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var veterinarian = With(VeterinarianFactory.Valid(), id: 0);

        var result = Validate(veterinarian);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Veterinarian.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Veterinarian.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Veterinarian.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void Cpf_Invalido(string cpf)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), cpf: cpf));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Veterinarian.Cpf));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("12345678901")]
    public void Crmv_Invalido(string crmv)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), crmv: crmv));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Veterinarian.Crmv));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InstitutionId_MenorOuIgualAZero_EInvalidoComMensagemCerta(int institutionId)
    {
        var result = Validate(With(VeterinarianFactory.Valid(), institutionId: institutionId));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(Veterinarian.InstitutionId) &&
            e.ErrorMessage == "O id da instituição deve ser maior que zero.");
    }

    private static Veterinarian With(
        Veterinarian vet,
        int? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        string? cpf = null,
        string? crmv = null,
        int? institutionId = null) =>
        new(
            id: id ?? vet.Id,
            name: name ?? vet.Name,
            email: email ?? vet.Email,
            password: password ?? vet.Password,
            cpf: cpf ?? vet.Cpf,
            crmv: crmv ?? vet.Crmv,
            institutionId: institutionId ?? vet.InstitutionId);
}