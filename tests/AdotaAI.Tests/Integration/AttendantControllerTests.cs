using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class AttendantControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public AttendantControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreateAttendantDto Dto(
        int institutionId,
        string name = "Mariana Costa",
        string email = "mariana.atend@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901") =>
        new(name, email, password, cpf, institutionId);

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EAtendentePersistido()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdFromPost = await response.Content.ReadFromJsonAsync<AttendantResponseDto>(JsonOptions);
        Assert.NotNull(createdFromPost);
        Assert.True(createdFromPost.Id > 0);

        var created = await _api.Client.GetFromJsonAsync<AttendantResponseDto>(
            $"/api/Attendant/{createdFromPost.Id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(createdFromPost.Id, created.Id);
        Assert.Equal("mariana.atend@email.com", created.Email);
        Assert.Equal(institutionId, created.InstitutionId);
    }

    [Fact]
    public async Task Post_ComCamposObrigatoriosInvalidos_Retorna400ComMensagensCertas()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Attendant",
            Dto(institutionId: 0, name: "", email: "email-invalido", password: "1", cpf: "123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotNull(body.Errors);
        Assert.Contains("O id da instituição deve ser maior que zero.", body.Errors);
    }

    [Fact]
    public async Task Post_ComInstitutionIdInexistente_Retorna400()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId: 9999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComEmailDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId, cpf: "99999999999"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComCpfDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId, email: "outro@email.com"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPorInstitutionId_RetornaApenasAtendentesDaInstituicao()
    {
        await _api.ResetDatabaseAsync();
        var inst1 = await _api.SeedInstitutionAsync("abrigo1@email.com");
        var inst2 = await _api.SeedInstitutionAsync("abrigo2@email.com");

        await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(inst1, email: "a1@email.com", cpf: "11111111111"));
        await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(inst2, email: "a2@email.com", cpf: "22222222222"));

        var list = await _api.Client.GetFromJsonAsync<List<AttendantResponseDto>>(
            $"/api/Attendant/institution/{inst1}", JsonOptions);

        Assert.NotNull(list);
        Assert.Single(list);
        Assert.Equal(inst1, list[0].InstitutionId);
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.GetAsync("/api/Attendant/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Atendente não encontrado.", body.Message);
    }

    [Fact]
    public async Task Put_ComAtendenteInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/Attendant/9999",
            new UpdateAttendantDto("Mariana Costa", "mariana@email.com", "senhaSegura123", "12345678901", 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComAtendenteInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.DeleteAsync("/api/Attendant/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostaDoEndpoint_NaoExpoeSenha()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var post = await _api.Client.PostAsJsonAsync("/api/Attendant", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var json = await _api.Client.GetStringAsync("/api/Attendant");

        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("senhaSegura123", json);
    }

    private sealed record ProblemBody(string Message, List<string>? Errors);
}