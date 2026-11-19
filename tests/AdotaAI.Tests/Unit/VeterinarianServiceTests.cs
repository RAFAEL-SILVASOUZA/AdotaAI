using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Tests.Fakes;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class VeterinarianServiceTests
{
    private readonly FakeVeterinarianRepository _repository = new();
    private readonly VeterinarianService _service;

    public VeterinarianServiceTests()
    {
        _service = new VeterinarianService(_repository);
    }

    private static CreateVeterinarianDto CreateDto(
        string name = "Dra. Ana Paula",
        string email = "ana.vet@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901",
        string crmv = "12345-SP",
        int institutionId = 1) =>
        new(name, email, password, cpf, crmv, institutionId);

    [Fact]
    public async Task AddAsync_ComDadosValidos_SalvaVeterinarioEDevolveDtoSemSenha()
    {
        var result = await _service.AddAsync(CreateDto());

        Assert.Equal(1, result.Id);
        Assert.Equal("ana.vet@email.com", result.Email);
        Assert.Equal("12345-SP", result.Crmv);
        Assert.Single(_repository.Veterinarians);
        Assert.Equal(1, _repository.SaveChangesCalls);
        Assert.Null(typeof(VeterinarianResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task AddAsync_ComDadosInvalidos_LancaValidationFailedException()
    {
        var dto = CreateDto(name: "", email: "email-invalido", password: "1", cpf: "123", crmv: "", institutionId: 0);

        await Assert.ThrowsAsync<ValidationFailedException>(() => _service.AddAsync(dto));
        Assert.Equal(0, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task AddAsync_ComEmailDuplicado_LancaInvalidOperationException()
    {
        await _service.AddAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddAsync(CreateDto(cpf: "99999999999", crmv: "99999-SP")));

        Assert.Equal("Já existe um funcionário cadastrado com este e-mail.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_ComCpfDuplicado_LancaInvalidOperationException()
    {
        await _service.AddAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddAsync(CreateDto(email: "outro@email.com", crmv: "99999-SP")));

        Assert.Equal("Já existe um funcionário cadastrado com este CPF.", ex.Message);
    }

    [Fact]
    public async Task AddAsync_ComCrmvDuplicado_LancaInvalidOperationException()
    {
        await _service.AddAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.AddAsync(CreateDto(email: "outro@email.com", cpf: "99999999999")));

        Assert.Equal("Já existe um veterinário cadastrado com este CRMV.", ex.Message);
    }

    [Fact]
    public async Task GetByIdAsync_ComVeterinarioInexistente_DevolveNull()
    {
        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ComVeterinarioExistente_PersisteAlteracoes()
    {
        var created = await _service.AddAsync(CreateDto());

        var dto = new UpdateVeterinarianDto(
            "Dra. Ana Atualizada", "ana.vet@email.com", "senhaSegura123", "12345678901", "12345-SP", 1);

        Assert.True(await _service.UpdateAsync(created.Id, dto));

        var saved = await _service.GetByIdAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal("Dra. Ana Atualizada", saved.Name);
    }

    [Fact]
    public async Task UpdateAsync_ComVeterinarioInexistente_DevolveFalse()
    {
        var dto = new UpdateVeterinarianDto(
            "Dra. Ana", "ana.vet@email.com", "senhaSegura123", "12345678901", "12345-SP", 1);

        var updated = await _service.UpdateAsync(99, dto);

        Assert.False(updated);
    }

    [Fact]
    public async Task DeleteAsync_ComVeterinarioExistente_RemoveVeterinario()
    {
        var created = await _service.AddAsync(CreateDto());

        Assert.True(await _service.DeleteAsync(created.Id));
        Assert.Empty(_repository.Veterinarians);
    }

    [Fact]
    public async Task DeleteAsync_ComVeterinarioInexistente_DevolveFalse()
    {
        Assert.False(await _service.DeleteAsync(99));
    }
}