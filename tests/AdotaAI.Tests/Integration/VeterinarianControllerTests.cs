using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class VeterinarianControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public VeterinarianControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreateVeterinarianDto Dto(
        int institutionId,
        string name = "Dra. Ana Paula",
        string email = "ana.vet@email.com",
        string password = "senhaSegura123",
        string cpf = "12345678901",
        string crmv = "12345-SP") =>
        new(name, email, password, cpf, crmv, institutionId);

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EVeterinarioPersistido()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdFromPost = await response.Content.ReadFromJsonAsync<VeterinarianResponseDto>(JsonOptions);
        Assert.NotNull(createdFromPost);
        Assert.True(createdFromPost.Id > 0);

        var created = await _api.Client.GetFromJsonAsync<VeterinarianResponseDto>(
            $"/api/Veterinarian/{createdFromPost.Id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(createdFromPost.Id, created.Id);
        Assert.Equal("ana.vet@email.com", created.Email);
        Assert.Equal("12345-SP", created.Crmv);
    }

    [Fact]
    public async Task Post_ComCamposObrigatoriosInvalidos_Retorna400ComMensagensCertas()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian",
            Dto(institutionId: 0, name: "", email: "email-invalido", password: "1", cpf: "123", crmv: ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.NotNull(body.Errors);
        Assert.Contains("O CRMV do veterinário não pode ser vazio.", body.Errors);
        Assert.Contains("O id da instituição deve ser maior que zero.", body.Errors);
    }

    [Fact]
    public async Task Post_ComInstitutionIdInexistente_Retorna400()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId: 9999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComEmailDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian",
            Dto(institutionId, cpf: "99999999999", crmv: "99999-SP"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComCpfDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian",
            Dto(institutionId, email: "outro@email.com", crmv: "99999-SP"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_ComCrmvDuplicado_Retorna400()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Veterinarian",
            Dto(institutionId, email: "outro@email.com", cpf: "99999999999"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.GetAsync("/api/Veterinarian/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Veterinário não encontrado.", body.Message);
    }

    [Fact]
    public async Task Put_ComVeterinarioInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/Veterinarian/9999",
            new UpdateVeterinarianDto("Dra. Ana", "ana@email.com", "senhaSegura123", "12345678901", "12345-SP", 1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComVeterinarioInexistente_Retorna404()
    {
        await _api.ResetDatabaseAsync();

        var response = await _api.Client.DeleteAsync("/api/Veterinarian/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostaDoEndpoint_NaoExpoeSenha()
    {
        await _api.ResetDatabaseAsync();
        var institutionId = await _api.SeedInstitutionAsync();
        var post = await _api.Client.PostAsJsonAsync("/api/Veterinarian", Dto(institutionId));
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var json = await _api.Client.GetStringAsync("/api/Veterinarian");

        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("senhaSegura123", json);
    }

    private sealed record ProblemBody(string Message, List<string>? Errors);
}