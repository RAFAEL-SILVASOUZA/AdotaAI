using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class InstitutionServiceTests
{
    private readonly FakeInstitutionRepository _repository = new();
    private readonly IValidator<Institution> _validator = new InstitutionValidator();
    private readonly InstitutionService _service;

    public InstitutionServiceTests()
    {
        _service = new InstitutionService(_repository, _validator);
    }

    private static CreateInstitutionDto CreateDto(
        string name = "Instituição Feliz",
        string email = "instituicao@email.com",
        string phone = "44999999999",
        string document = "12345678987654",
        string photo = "foto.png",
        string password = "senhaSegura123",
        string description = "Instituição dedicada ao resgate de animais.",
        string operatingHours = "08:00 às 18:00",
        string address = "Rua das Flores, 123") =>
        new(name, email, phone, document, photo, password, description, operatingHours, address);

    private static Institution ValidEntity(int id = 0, string email = "instituicao@email.com") =>
        new(id, "Instituição Feliz", email, "44999999999", "12345678987654",
            "foto.png", "senhaSegura123", "Instituição dedicada ao resgate de animais.",
            "08:00 às 18:00", "Rua das Flores, 123");

    [Fact]
    public async Task CreateAsync_ComDadosValidos_SalvaInstituicaoEUsaIdGeradoPeloBanco()
    {
        var id = await _service.CreateAsync(CreateDto());

        Assert.Equal(1, id);
        Assert.Single(_repository.Institutions);
        Assert.Equal(1, _repository.Institutions.First().Id);
        Assert.Equal(1, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ComCamposObrigatoriosInvalidos_LancaValidationFailedExceptionComMensagensCertas()
    {
        var dto = CreateDto(
            name: "",
            email: "email-invalido",
            phone: "123",
            document: "",
            photo: "",
            password: "123",
            description: "curta",
            operatingHours: "",
            address: "Rua");

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() => _service.CreateAsync(dto));

        Assert.DoesNotContain("O id da instituição deve ser maior que zero.", ex.Errors);
        Assert.Contains("O nome da instituição não pode ser vazio.", ex.Errors);
        Assert.Contains("O email da instituição não é válido.", ex.Errors);
        Assert.Contains("O telefone da instituição deve ter pelo menos 10 caracteres.", ex.Errors);
        Assert.Contains("O documento da instituição não pode ser vazio.", ex.Errors);
        Assert.Contains("A foto da instituição não pode ser vazia.", ex.Errors);
        Assert.Contains("A senha da instituição deve ter pelo menos 6 caracteres.", ex.Errors);
        Assert.Contains("A descrição da instituição deve ter pelo menos 10 caracteres.", ex.Errors);
        Assert.Contains("O horário de funcionamento da instituição não pode ser vazio.", ex.Errors);
        Assert.Contains("O endereço da instituição deve ter pelo menos 5 caracteres.", ex.Errors);
        Assert.Equal(0, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ComEmailJaCadastrado_LancaValidationFailedException()
    {
        await _service.CreateAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(
            () => _service.CreateAsync(CreateDto(name: "Outra Instituição")));

        Assert.Contains("O email informado já está cadastrado.", ex.Errors);
        Assert.Single(_repository.Institutions);
    }

    [Fact]
    public async Task GetByIdAsync_ComInstituicaoExistente_DevolveDtoSemSenha()
    {
        await _repository.AddAsync(ValidEntity());

        var dto = await _service.GetByIdAsync(1);

        Assert.Equal(1, dto.Id);
        Assert.Equal("instituicao@email.com", dto.Email);
        Assert.Null(typeof(InstitutionResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task GetByIdAsync_ComInstituicaoInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));

        Assert.Equal("Instituição com id 99 não encontrada.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ComInstituicaoExistente_PersisteAlteracoes()
    {
        await _repository.AddAsync(ValidEntity());

        var dto = new UpdateInstitutionDto(
            1, "Instituição Atualizada", "instituicao@email.com", "44999999999",
            "12345678987654", "foto.png", "senhaSegura123",
            "Descrição atualizada da instituição.", "09:00 às 19:00", "Rua Nova, 456");

        Assert.True(await _service.UpdateAsync(dto));

        var saved = await _service.GetByIdAsync(1);
        Assert.Equal("Instituição Atualizada", saved.Name);
        Assert.Equal("Rua Nova, 456", saved.Address);
    }

    [Fact]
    public async Task UpdateAsync_ComInstituicaoInexistente_LancaNotFoundException()
    {
        var dto = new UpdateInstitutionDto(
            99, "Instituição Feliz", "instituicao@email.com", "44999999999",
            "12345678987654", "foto.png", "senhaSegura123",
            "Instituição dedicada ao resgate de animais.", "08:00 às 18:00", "Rua das Flores, 123");

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(dto));

        Assert.Equal("Instituição com id 99 não encontrada.", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_ComInstituicaoExistente_RemoveInstituicao()
    {
        await _repository.AddAsync(ValidEntity());

        Assert.True(await _service.DeleteAsync(1));
        Assert.Empty(_repository.Institutions);
    }

    [Fact]
    public async Task DeleteAsync_ComInstituicaoInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));

        Assert.Equal("Instituição com id 99 não encontrada.", ex.Message);
    }
}