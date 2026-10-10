using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using AdotaAI.Domain;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class UserControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public UserControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreateUserDto Dto(
        string name = "Maria Silva",
        string email = "maria@email.com",
        string phone = "11999998888",
        int age = 25,
        string photo = "perfil.jpg",
        string password = "senhaSegura123",
        Sex gender = Sex.Female,
        string address = "Rua das Flores, 123") =>
        new(name, email, phone, age, photo, password, gender, address);

    private async Task<int> CreateAsync(CreateUserDto dto)
    {
        var response = await _api.Client.PostAsJsonAsync("/api/User", dto);
        return await response.Content.ReadFromJsonAsync<int>(JsonOptions);
    }

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EUsuarioPersistido()
    {
        await _api.ClearUsersAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/User", Dto());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var id = await response.Content.ReadFromJsonAsync<int>(JsonOptions);
        // O id vem do AUTOINCREMENT do SQLite, então só se pode afirmar que foi gerado.
        Assert.True(id > 0);

        var created = await _api.Client.GetFromJsonAsync<UserResponseDto>($"/api/User/{id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(id, created.Id);
        Assert.Equal("maria@email.com", created.Email);
    }

    [Fact]
    public async Task Post_ComDadosInvalidos_Retorna400ComMensagensEmPortugues()
    {
        await _api.ClearUsersAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/User",
            Dto(name: "A", email: "email-invalido", phone: "123", age: 17, photo: "",
                password: "123", gender: (Sex)9, address: "Rua"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Dados do usuário são inválidos.", body.Message);
        Assert.Equal(8, body.Errors.Count);
        Assert.Contains("O email do usuário não é válido.", body.Errors);
    }

    [Fact]
    public async Task Post_ComEmailDuplicado_Retorna400()
    {
        await _api.ClearUsersAsync();
        await CreateAsync(Dto());

        var response = await _api.Client.PostAsJsonAsync("/api/User", Dto(name: "Outra Pessoa"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Contains("O email informado já está cadastrado.", body.Errors);
    }

    [Fact]
    public async Task GetLista_Retorna200ComUsuariosOrdenados()
    {
        await _api.ClearUsersAsync();
        await CreateAsync(Dto(email: "um@email.com"));
        await CreateAsync(Dto(email: "dois@email.com"));

        var response = await _api.Client.GetAsync("/api/User");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>(JsonOptions);
        Assert.NotNull(users);
        Assert.Equal(2, users.Count);
        // Ids vêm do AUTOINCREMENT: o que se pode afirmar é a ordem crescente.
        Assert.Equal(users.Select(u => u.Id).ToList(), users.Select(u => u.Id).OrderBy(i => i).ToList());
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ClearUsersAsync();

        var response = await _api.Client.GetAsync("/api/User/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Usuário com id 9999 não encontrado.", body.Message);
    }

    [Fact]
    public async Task Put_ComUsuarioExistente_Retorna204EAlteracaoPersistida()
    {
        await _api.ClearUsersAsync();
        var id = await CreateAsync(Dto());

        var response = await _api.Client.PutAsJsonAsync("/api/User",
            new UpdateUserDto(id, "Maria Silva Sousa", "maria@email.com", "11999998888", 26,
                "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var updated = await _api.Client.GetFromJsonAsync<UserResponseDto>($"/api/User/{id}", JsonOptions);
        Assert.NotNull(updated);
        Assert.Equal("Maria Silva Sousa", updated.Name);
        Assert.Equal(26, updated.Age);
    }

    [Fact]
    public async Task Put_ComUsuarioInexistente_Retorna404()
    {
        await _api.ClearUsersAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/User",
            new UpdateUserDto(9999, "Maria Silva", "maria@email.com", "11999998888", 25,
                "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_ComDadosInvalidos_Retorna400EAlteraNada()
    {
        await _api.ClearUsersAsync();
        var id = await CreateAsync(Dto());

        var response = await _api.Client.PutAsJsonAsync("/api/User",
            new UpdateUserDto(id, "A", "maria@email.com", "11999998888", 25,
                "perfil.jpg", "senhaSegura123", Sex.Female, "Rua das Flores, 123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var unchanged = await _api.Client.GetFromJsonAsync<UserResponseDto>($"/api/User/{id}", JsonOptions);
        Assert.NotNull(unchanged);
        Assert.Equal("Maria Silva", unchanged.Name);
    }

    [Fact]
    public async Task Delete_ComUsuarioExistente_Retorna204EUsuarioSome()
    {
        await _api.ClearUsersAsync();
        var id = await CreateAsync(Dto());

        var response = await _api.Client.DeleteAsync($"/api/User/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var afterDelete = await _api.Client.GetAsync($"/api/User/{id}");
        Assert.Equal(HttpStatusCode.NotFound, afterDelete.StatusCode);
    }

    [Fact]
    public async Task Delete_ComUsuarioInexistente_Retorna404()
    {
        await _api.ClearUsersAsync();

        var response = await _api.Client.DeleteAsync("/api/User/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostaDoEndpoint_NaoExpoeSenha()
    {
        await _api.ClearUsersAsync();
        await CreateAsync(Dto());

        var json = await _api.Client.GetStringAsync("/api/User");

        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("senhaSegura123", json);
    }

    // Formato do corpo de erro do middleware do Program.cs.
    private sealed record ProblemBody(string Message, List<string> Errors);
}
