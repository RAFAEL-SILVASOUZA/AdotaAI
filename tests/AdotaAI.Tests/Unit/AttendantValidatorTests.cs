using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class AttendantValidatorTests
{
    private readonly AttendantValidator _validator = new();

    private ValidationResult Validate(Attendant attendant) => _validator.Validate(attendant);

    [Fact]
    public void AttendantValido_PassaEmTodasAsRegras()
    {
        var result = Validate(AttendantFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var attendant = With(AttendantFactory.Valid(), id: 0);

        var result = Validate(attendant);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(AttendantFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Attendant.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(AttendantFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Attendant.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(AttendantFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Attendant.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void Cpf_Invalido(string cpf)
    {
        var result = Validate(With(AttendantFactory.Valid(), cpf: cpf));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Attendant.Cpf));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InstitutionId_MenorOuIgualAZero_EInvalidoComMensagemCerta(int institutionId)
    {
        var result = Validate(With(AttendantFactory.Valid(), institutionId: institutionId));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.PropertyName == nameof(Attendant.InstitutionId) &&
            e.ErrorMessage == "O id da instituição deve ser maior que zero.");
    }

    private static Attendant With(
        Attendant attendant,
        int? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        string? cpf = null,
        int? institutionId = null) =>
        new(
            id: id ?? attendant.Id,
            name: name ?? attendant.Name,
            email: email ?? attendant.Email,
            password: password ?? attendant.Password,
            cpf: cpf ?? attendant.Cpf,
            institutionId: institutionId ?? attendant.InstitutionId);
}