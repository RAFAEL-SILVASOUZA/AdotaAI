using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class EmployeeServiceTests
{
    private readonly FakeEmployeeRepository _repository = new();
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _service = new EmployeeService(_repository, new EmployeeValidator());
    }

    private static CreateEmployeeDto CreateDto(
        string name = "Carlos Souza",
        string email = "carlos.func@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901") =>
        new(name, email, password, cpf);

    [Fact]
    public async Task AddAsync_ComDadosValidos_SalvaFuncionarioEDevolveDtoSemSenha()
    {
        var result = await _service.AddAsync(CreateDto());

        Assert.Equal(1, result.Id);
        Assert.Equal("carlos.func@email.com", result.Email);
        Assert.Equal("12345678901", result.Cpf);
        Assert.Single(_repository.Employees);
        Assert.Equal(1, _repository.SaveChangesCalls);
        Assert.Null(typeof(EmployeeResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task AddAsync_ComDadosInvalidos_LancaValidationFailedException()
    {
        var dto = CreateDto(name: "", email: "email-invalido", password: "1", cpf: "123");

        await Assert.ThrowsAsync<ValidationFailedException>(() => _service.AddAsync(dto));
        Assert.Equal(0, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task AddAsync_ComEmailDuplicado_LancaInvalidOperationException()
    {
        await _service.AddAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddAsync(CreateDto(cpf: "99999999999")));

        Assert.Equal("Já existe um funcionário cadastrado com este e-mail.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_ComCpfDuplicado_LancaInvalidOperationException()
    {
        await _service.AddAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddAsync(CreateDto(email: "outro@email.com")));

        Assert.Equal("Já existe um funcionário cadastrado com este CPF.", ex.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ComFuncionarioInexistente_DevolveNull()
    {
        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ComFuncionarioExistente_PersisteAlteracoes()
    {
        var created = await _service.AddAsync(CreateDto());

        var dto = new UpdateEmployeeDto(
            "Carlos Atualizado", "carlos.func@email.com", "senhaSegura123", "12345678901");

        Assert.True(await _service.UpdateAsync(created.Id, dto));

        var saved = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal("Carlos Atualizado", saved.Name);
    }

    [Fact]
    public async Task UpdateAsync_ComFuncionarioInexistente_DevolveFalse()
    {
        var dto = new UpdateEmployeeDto(
            "Carlos Souza", "carlos.func@email.com", "senhaSegura123", "12345678901");

        var updated = await _service.UpdateAsync(99, dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteAsync_ComFuncionarioExistente_RemoveFuncionario()
    {
        var created = await _service.AddAsync(CreateDto());

        Assert.True(await _service.DeleteAsync(created.Id));
        Assert.Empty(_repository.Employees);
    }

    [Fact]
    public async Task DeleteAsync_ComFuncionarioInexistente_DevolveFalse()
    {
        Assert.False(await _service.DeleteAsync(99));
    }
}