using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class AdmControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public AdmControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreateAdmDto Dto(
        string name = "Fernanda Lima",
        string email = "fernanda.adm@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901") =>
        new(name, email, password, cpf);

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EAdmPersistido()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Adm", Dto());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdFromPost = await response.Content.ReadFromJsonAsync<AdmResponseDto>(JsonOptions);
        Assert.NotNull(createdFromPost);
        Assert.True(createdFromPost.Id > 0);

        var created = await _api.Client.GetFromJsonAsync<AdmResponseDto>(
            $"/api/Adm/{createdFromPost.Id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(createdFromPost.Id, created.Id);
        Assert.Equal("fernanda.adm@email.com", created.Email);
    }

    [Fact]
    public async Task Post_ComCamposObrigatoriosInvalidos_Retorna400ComMensagensCertas()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Adm",
            Dto(name: "", email: "email-invalido", password: "1", cpf: "123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotNull(body.Errors);
        Assert.Contains("O nome do funcionário não pode ser vazio.", body.Errors);
    }

    [Fact]
    public async Task Post_ComEmailDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Adm", Dto());
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Adm", Dto(cpf: "99999999999"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComCpfDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Adm", Dto());
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Adm", Dto(email: "outro@email.com"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.GetAsync("/api/Adm/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Administrador não encontrado.", body.Message);
    }

    [Fact]
    public async Task Put_ComAdmInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/Adm/9999",
            new UpdateAdmDto("Fernanda Lima", "fernanda@email.com", "senhaSegura123", "12345678901"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComAdmInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.DeleteAsync("/api/Adm/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostaDoEndpoint_NaoExpoeSenha()
    {
        await _api.ResetDatabaseAsync();
        var post = await _api.Client.PostAsJsonAsync("/api/Adm", Dto());
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var json = await _api.Client.GetStringAsync("/api/Adm");

        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("senhaSegura123", json);
    }

    private sealed record ProblemBody(string Message, List<string>? Errors);
}