using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class UserValidatorTests
{
    private readonly UserValidator _validator = new();

    private ValidationResult Validate(User user) => _validator.Validate(user);

    [Fact]
    public void UserValido_PassaEmTodasAsRegras()
    {
        var result = Validate(UserFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var user = new User(
            id: 0,
            name: "Maria Silva",
            email: "maria@email.com",
            phone: "11999998888",
            age: 25,
            photo: "perfil.jpg",
            password: "senhaSegura123",
            gender: Sex.Female,
            address: "Rua das Flores, 123");

        var result = Validate(user);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(UserFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Name));
    }

    [Fact]
    public void Nome_MaiorQue70Caracteres_EInvalido()
    {
        var result = Validate(With(UserFactory.Valid(), name: new string('a', 71)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("maria@@email.com")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(UserFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("1234567890123456")]
    public void Telefone_Invalido(string phone)
    {
        var result = Validate(With(UserFactory.Valid(), phone: phone));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Phone));
    }

    [Theory]
    [InlineData(17)]
    [InlineData(101)]
    [InlineData(-1)]
    public void Idade_Invalida(int age)
    {
        var result = Validate(With(UserFactory.Valid(), age: age));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Age));
    }

    [Fact]
    public void Idade_NosLimites_EValida()
    {
        Assert.True(Validate(With(UserFactory.Valid(), age: 18)).IsValid);
        Assert.True(Validate(With(UserFactory.Valid(), age: 100)).IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(UserFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Password));
    }

    [Fact]
    public void Senha_MaiorQue25Caracteres_EInvalida()
    {
        var result = Validate(With(UserFactory.Valid(), password: new string('a', 26)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("Rua")]
    public void Endereco_Invalido(string address)
    {
        var result = Validate(With(UserFactory.Valid(), address: address));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Address));
    }

    [Fact]
    public void Foto_Vazia_EInvalida()
    {
        var result = Validate(With(UserFactory.Valid(), photo: ""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Photo));
    }

    [Theory]
    [InlineData((Sex)0)]
    [InlineData((Sex)4)]
    [InlineData((Sex)(-1))]
    public void Genero_ForaDoEnum_EInvalido(Sex gender)
    {
        var result = Validate(With(UserFactory.Valid(), gender: gender));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Gender));
    }

    [Fact]
    public void MensagensDeErro_SaoEmPortugues()
    {
        var result = Validate(With(UserFactory.Valid(), name: "A", email: "email-invalido"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O nome do usuário deve ter pelo menos 2 caracteres.");
        Assert.Contains(result.Errors, e => e.ErrorMessage == "O email do usuário não é válido.");
    }

    // Copia a entidade com os campos alterados (private set impede atribuição direta).
    private static User With(
        User user,
        string? name = null,
        string? email = null,
        string? phone = null,
        int? age = null,
        string? photo = null,
        string? password = null,
        Sex? gender = null,
        string? address = null) =>
        new(
            id: user.Id,
            name: name ?? user.Name,
            email: email ?? user.Email,
            phone: phone ?? user.Phone,
            age: age ?? user.Age,
            photo: photo ?? user.Photo,
            password: password ?? user.Password,
            gender: gender ?? user.Gender,
            address: address ?? user.Address);
}
