using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class AdmValidatorTests
{
    private readonly AdmValidator _validator = new();

    private ValidationResult Validate(Adm adm) => _validator.Validate(adm);

    [Fact]
    public void AdmValido_PassaEmTodasAsRegras()
    {
        var result = Validate(AdmFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var adm = With(AdmFactory.Valid(), id: 0);

        var result = Validate(adm);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(AdmFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Name));
    }

    [Fact]
    public void Nome_MaiorQue70Caracteres_EInvalido()
    {
        var result = Validate(With(AdmFactory.Valid(), name: new string('a', 71)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("fernanda@@email.com")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(AdmFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(AdmFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Password));
    }

    [Fact]
    public void Senha_MaiorQue25Caracteres_EInvalida()
    {
        var result = Validate(With(AdmFactory.Valid(), password: new string('a', 26)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123456789012")]
    public void Cpf_Invalido(string cpf)
    {
        var result = Validate(With(AdmFactory.Valid(), cpf: cpf));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Adm.Cpf));
    }

    private static Adm With(
        Adm adm,
        int? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        string? cpf = null) =>
        new(
            id: id ?? adm.Id,
            name: name ?? adm.Name,
            email: email ?? adm.Email,
            password: password ?? adm.Password,
            cpf: cpf ?? adm.Cpf);
}