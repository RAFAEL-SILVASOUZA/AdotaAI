using AdotaAI.Application;
using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using AdotaAI.Tests.Fakes;
using AdotaAI.Validation;
using FluentValidation;
using Xunit;

namespace AdotaAI.Tests.Unit;

public class UserServiceTests
{
    private readonly FakeUserRepository _repository = new();
    private readonly IValidator<User> _validator = new UserValidator();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_repository, _validator);
    }

    private static CreateUserDto CreateDto(
        string name = "Maria Silva",
        string email = "maria@email.com",
        string phone = "11999998888",
        int age = 25,
        string photo = "perfil.jpg",
        string password = "senhaSegura123",
        Sex gender = Sex.Female,
        string address = "Rua das Flores, 123") =>
        new(name, email, phone, age, photo, password, gender, address);

    [Fact]
    public async Task CreateAsync_ComDadosValidos_SalvaUsuarioEUsaIdGeradoPeloBanco()
    {
        var id = await _service.CreateAsync(CreateDto());

        Assert.Equal(1, id);
        Assert.Single(_repository.Users);
        Assert.Equal(1, _repository.Users.First().Id);
        Assert.Equal(1, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ComDadosInvalidos_LancaValidationFailedExceptionComMensagens()
    {
        var dto = CreateDto(name: "A", email: "email-invalido", phone: "123", age: 17, photo: "", password: "123", gender: (Sex)9, address: "Rua");

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() => _service.CreateAsync(dto));

        Assert.Equal(8, ex.Errors.Count);
        Assert.Contains("O nome do usuário deve ter pelo menos 2 caracteres.", ex.Errors);
        Assert.Contains("O email do usuário não é válido.", ex.Errors);
        Assert.Contains("A idade do usuário deve estar entre 18 e 100 anos.", ex.Errors);
        Assert.Equal(0, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task CreateAsync_ComEmailJaCadastrado_LancaValidationFailedException()
    {
        await _service.CreateAsync(CreateDto());

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(
            () => _service.CreateAsync(CreateDto(name: "Outra Pessoa")));

        Assert.Contains("O email informado já está cadastrado.", ex.Errors);
        Assert.Single(_repository.Users);
    }

    [Fact]
    public async Task GetByIdAsync_ComUsuarioExistente_DevolveDtoSemSenha()
    {
        var id = await _service.CreateAsync(CreateDto());

        var dto = await _service.GetByIdAsync(id);

        Assert.Equal(id, dto.Id);
        Assert.Equal("maria@email.com", dto.Email);
        Assert.Equal(Sex.Female, dto.Gender);
        // O response DTO não tem a propriedade Password: a senha não sai da Application.
        Assert.Null(typeof(UserResponseDto).GetProperty("Password"));
    }

    [Fact]
    public async Task GetByIdAsync_ComUsuarioInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(99));

        Assert.Equal("Usuário com id 99 não encontrado.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_DevolveListaOrdenadaPorId()
    {
        await _service.CreateAsync(CreateDto(email: "um@email.com"));
        await _service.CreateAsync(CreateDto(email: "dois@email.com"));

        var dtos = await _service.GetAllAsync();

        Assert.Equal(2, dtos.Count);
        Assert.Equal(new[] { 1, 2 }, dtos.Select(d => d.Id).ToList());
    }

    [Fact]
    public async Task GetAllAsync_SemUsuarios_DevolveListaVazia()
    {
        var dtos = await _service.GetAllAsync();

        Assert.Empty(dtos);
    }

    [Fact]
    public async Task UpdateAsync_ComUsuarioExistente_PersisteAlteracoes()
    {
        var id = await _service.CreateAsync(CreateDto());

        var dto = new UpdateUserDto(id, "Maria Silva Sousa", "maria@email.com", "11999998888", 26,
            "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123");

        Assert.True(await _service.UpdateAsync(dto));

        var saved = await _service.GetByIdAsync(id);
        Assert.Equal("Maria Silva Sousa", saved.Name);
        Assert.Equal(26, saved.Age);
    }

    [Fact]
    public async Task UpdateAsync_ComUsuarioInexistente_LancaNotFoundException()
    {
        var dto = new UpdateUserDto(99, "Maria Silva", "maria@email.com", "11999998888", 25,
            "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123");

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(dto));

        Assert.Equal("Usuário com id 99 não encontrado.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_ComDadosInvalidos_LancaValidationFailedExceptionESalvaNada()
    {
        var id = await _service.CreateAsync(CreateDto());
        var callsBefore = _repository.SaveChangesCalls;

        var dto = new UpdateUserDto(id, "A", "maria@email.com", "11999998888", 25,
            "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123");

        var ex = await Assert.ThrowsAsync<ValidationFailedException>(() => _service.UpdateAsync(dto));

        Assert.Contains("O nome do usuário deve ter pelo menos 2 caracteres.", ex.Errors);
        Assert.Equal(callsBefore, _repository.SaveChangesCalls);
    }

    [Fact]
    public async Task DeleteAsync_ComUsuarioExistente_RemoveUsuario()
    {
        var id = await _service.CreateAsync(CreateDto());

        Assert.True(await _service.DeleteAsync(id));
        Assert.Empty(_repository.Users);
    }

    [Fact]
    public async Task DeleteAsync_ComUsuarioInexistente_LancaNotFoundException()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(99));

        Assert.Equal("Usuário com id 99 não encontrado.", ex.Message);
    }
}
