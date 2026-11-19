using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Tests.Fakes;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class AdmServiceTests
{
    private readonly FakeAdmRepository _repository = new();
    private readonly AdmService _service;

    public AdmServiceTests()
    {
        _service = new AdmService(_repository);
    }

    private static CreateAdmDto CreateDto(
        string name = "Fernanda Lima",
        string email = "fernanda.adm@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901") =>
        new(name, email, password, cpf);

    [Fact]
    public async Task AddAsync_ComDadosValidos_SalvaAdmEDevolveDtoSemSenha()
    {
        var result = await _service.AddAsync(CreateDto());

        Assert.Equal(1, result.Id);
        Assert.Equal("fernanda.adm@email.com", result.Email);
        Assert.Equal("12345678901", result.Cpf);
        Assert.Single(_repository.Adms);
        Assert.Equal(1, _repository.SaveChangesCalls);
        Assert.Null(typeof(AdmResponseDto).GetProperty("Password"));
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
    public async Task GetByIdAsync_ComAdmInexistente_DevolveNull()
    {
        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ComAdmExistente_PersisteAlteracoes()
    {
        var created = await _service.AddAsync(CreateDto());

        var dto = new UpdateAdmDto(
            "Fernanda Atualizada", "fernanda.adm@email.com", "senhaSegura123", "12345678901");

        Assert.True(await _service.UpdateAsync(created.Id, dto));

        var saved = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal("Fernanda Atualizada", saved.Name);
    }

    [Fact]
    public async Task UpdateAsync_ComAdmInexistente_DevolveFalse()
    {
        var dto = new UpdateAdmDto(
            "Fernanda Lima", "fernanda.adm@email.com", "senhaSegura123", "12345678901");

        var updated = await _service.UpdateAsync(99, dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteAsync_ComAdmExistente_RemoveAdm()
    {
        var created = await _service.AddAsync(CreateDto());

        Assert.True(await _service.DeleteAsync(created.Id));
        Assert.Empty(_repository.Adms);
    }

    [Fact]
    public async Task DeleteAsync_ComAdmInexistente_DevolveFalse()
    {
        Assert.False(await _service.DeleteAsync(99));
    }
}