using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation.Results;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class EmployeeValidatorTests
{
    private readonly EmployeeValidator _validator = new();

    private ValidationResult Validate(Employee employee) => _validator.Validate(employee);

    [Fact]
    public void EmployeeValido_PassaEmTodasAsRegras()
    {
        var result = Validate(EmployeeFactory.Valid());

        Assert.True(result.IsValid);
    }

    [Fact]
    public void IdZero_NaoEValidadoPorqueOBancoGeraOId()
    {
        var employee = With(EmployeeFactory.Valid(), id: 0);

        var result = Validate(employee);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nome_Invalido(string name)
    {
        var result = Validate(With(EmployeeFactory.Valid(), name: name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Name));
    }

    [Fact]
    public void Nome_MaiorQue70Caracteres_EInvalido()
    {
        var result = Validate(With(EmployeeFactory.Valid(), name: new string('a', 71)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    [InlineData("carlos@@email.com")]
    public void Email_Invalido(string email)
    {
        var result = Validate(With(EmployeeFactory.Valid(), email: email));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    public void Senha_Invalida(string password)
    {
        var result = Validate(With(EmployeeFactory.Valid(), password: password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Password));
    }

    [Fact]
    public void Senha_MaiorQue25Caracteres_EInvalida()
    {
        var result = Validate(With(EmployeeFactory.Valid(), password: new string('a', 26)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("123456789012")]
    public void Cpf_Invalido(string cpf)
    {
        var result = Validate(With(EmployeeFactory.Valid(), cpf: cpf));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(Employee.Cpf));
    }

    private static Employee With(
        Employee employee,
        int? id = null,
        string? name = null,
        string? email = null,
        string? password = null,
        string? cpf = null) =>
        new(
            id: id ?? employee.Id,
            name: name ?? employee.Name,
            email: email ?? employee.Email,
            password: password ?? employee.Password,
            cpf: cpf ?? employee.Cpf);
}