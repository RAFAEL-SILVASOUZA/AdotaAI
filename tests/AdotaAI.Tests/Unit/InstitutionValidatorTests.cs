using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class InstitutionValidatorTests
{
    private readonly InstitutionValidator _validator = new();

    private ValidationResult Validate(Institution institution) => _validator.Validate(institution);

    [Fact]
    public void InstitutionValida_PassaEmTodasAsRegras()
    {
        var result = Validate(InstitutionFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var institution = With(InstitutionFactory.Valid(), id: 0);

        var result = Validate(institution);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(InstitutionFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Name));
    }

    [Fact]
    public void Nome_MaiorQue100Caracteres_EInvalido()
    {
        var result = Validate(With(InstitutionFactory.Valid(), name: new string('a', 101)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("instituicao@@email.com")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(InstitutionFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123456")]
    public void Telefone_Invalido(string phone)
    {
        var result = Validate(With(InstitutionFactory.Valid(), phone: phone));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Phone));
    }

    [Fact]
    public void Documento_Vazio_EInvalido()
    {
        var result = Validate(With(InstitutionFactory.Valid(), document: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Document));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(InstitutionFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Password));
    }

    [Fact]
    public void Senha_MaiorQue25Caracteres_EInvalida()
    {
        var result = Validate(With(InstitutionFactory.Valid(), password: new string('a', 26)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Curta")]
    public void Descricao_Invalida(string description)
    {
        var result = Validate(With(InstitutionFactory.Valid(), description: description));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Description));
    }

    [Fact]
    public void HorarioFuncionamento_Vazio_EInvalido()
    {
        var result = Validate(With(InstitutionFactory.Valid(), operatingHours: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.OperatingHours));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Rua")]
    public void Endereco_Invalido(string address)
    {
        var result = Validate(With(InstitutionFactory.Valid(), address: address));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Address));
    }

    [Fact]
    public void Foto_Vazia_EInvalida()
    {
        var result = Validate(With(InstitutionFactory.Valid(), photo: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Institution.Photo));
    }

    private static Institution With(
        Institution institution,
        int? id = null,
        string? name = null,
        string? email = null,
        string? phone = null,
        string? document = null,
        string? photo = null,
        string? password = null,
        string? description = null,
        string? operatingHours = null,
        string? address = null) =>
        new(
            id: id ?? institution.Id,
            name: name ?? institution.Name,
            email: email ?? institution.Email,
            phone: phone ?? institution.Phone,
            document: document ?? institution.Document,
            photo: photo ?? institution.Photo,
            password: password ?? institution.Password,
            description: description ?? institution.Description,
            operatingHours: operatingHours ?? institution.OperatingHours,
            address: address ?? institution.Address);
}