using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class AttendantServiceTests
{
    private readonly FakeAttendantRepository _repository = new();
    private readonly AttendantService _service;

    public AttendantServiceTests()
    {
        _service = new AttendantService(_repository, new AttendantValidator());
    }

    private static CreateAttendantDto CreateDto(
        string name = "Mariana Costa",
        string email = "mariana.atend@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901",
        int institutionId = 1) =>
        new(name, email, password, cpf, institutionId);

    [Fact]
    public async Task AddAsync_ComDadosValidos_SalvaAtendenteEDevolveDtoSemSenha()
    {
        var result = await _service.AddAsync(CreateDto());

        Assert.Equal(1, result.Id);
        Assert.Equal("mariana.atend@email.com", result.Email);
        Assert.Equal(1, result.InstitutionId);
        Assert.Single(_repository.Attendants);
        Assert.Equal(1, _repository.SaveChangesCalls);
        Assert.Null(typeof(AttendantResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task AddAsync_ComDadosInvalidos_LancaValidationFailedException()
    {
        var dto = CreateDto(name: "", email: "email-invalido", password: "1", cpf: "123", institutionId: 0);

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
    public async Task GetByIdAsync_ComAtendenteInexistente_DevolveNull()
    {
        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByInstitutionIdAsync_FiltraAtendentesDaInstituicao()
    {
        await _service.AddAsync(CreateDto(email: "a1@email.com", cpf: "11111111111", institutionId: 10));
        await _service.AddAsync(CreateDto(email: "a2@email.com", cpf: "22222222222", institutionId: 20));

        var list = await _service.GetByInstitutionIdAsync(10);

        Assert.Single(list);
        Assert.Equal(10, list[0].InstitutionId);
    }

    [Fact]
    public async Task UpdateAsync_ComAtendenteExistente_PersisteAlteracoes()
    {
        var created = await _service.AddAsync(CreateDto());

        var dto = new UpdateAttendantDto(
            "Mariana Atualizada", "mariana.atend@email.com", "senhaSegura123", "12345678901", 1);

        Assert.True(await _service.UpdateAsync(created.Id, dto));

        var saved = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal("Mariana Atualizada", saved.Name);
    }

    [Fact]
    public async Task UpdateAsync_ComAtendenteInexistente_DevolveFalse()
    {
        var dto = new UpdateAttendantDto(
            "Mariana Costa", "mariana.atend@email.com", "senhaSegura123", "12345678901", 1);

        var updated = await _service.UpdateAsync(99, dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteAsync_ComAtendenteExistente_RemoveAtendente()
    {
        var created = await _service.AddAsync(CreateDto());

        Assert.True(await _service.DeleteAsync(created.Id));
        Assert.Empty(_repository.Attendants);
    }

    [Fact]
    public async Task DeleteAsync_ComAtendenteInexistente_DevolveFalse()
    {
        Assert.False(await _service.DeleteAsync(99));
    }
}