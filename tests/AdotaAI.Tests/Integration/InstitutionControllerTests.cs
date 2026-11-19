using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class InstitutionControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public InstitutionControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreateInstitutionDto Dto(
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

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EInstituicaoPersistida()
    {
        await _api.ClearInstitutionsAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Institution", Dto());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var id = await response.Content.ReadFromJsonAsync<int>(JsonOptions);
        Assert.True(id > 0);

        var created = await _api.Client.GetFromJsonAsync<InstitutionResponseDto>($"/api/Institution/{id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(id, created.Id);
        Assert.Equal("instituicao@email.com", created.Email);
    }

    [Fact]
    public async Task Post_ComCamposObrigatoriosInvalidos_Retorna400ComMensagensCertas()
    {
        await _api.ClearInstitutionsAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Institution",
            Dto(name: "", email: "email-invalido", phone: "123", document: "",
                photo: "", password: "123", description: "curta", operatingHours: "", address: "Rua"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.DoesNotContain("O id da instituição deve ser maior que zero.", body.Errors);
        Assert.Contains("O nome da instituição não pode ser vazio.", body.Errors);
        Assert.Contains("O email da instituição não é válido.", body.Errors);
        Assert.Contains("O telefone da instituição deve ter pelo menos 10 caracteres.", body.Errors);
        Assert.Contains("O documento da instituição não pode ser vazio.", body.Errors);
        Assert.Contains("A foto da instituição não pode ser vazia.", body.Errors);
        Assert.Contains("A senha da instituição deve ter pelo menos 6 caracteres.", body.Errors);
        Assert.Contains("A descrição da instituição deve ter pelo menos 10 caracteres.", body.Errors);
        Assert.Contains("O horário de funcionamento da instituição não pode ser vazio.", body.Errors);
        Assert.Contains("O endereço da instituição deve ter pelo menos 5 caracteres.", body.Errors);
    }

    [Fact]
    public async Task Post_ComEmailDuplicado_Retorna400()
    {
        await _api.ClearInstitutionsAsync();
        var first = await _api.Client.PostAsJsonAsync("/api/Institution", Dto());
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var response = await _api.Client.PostAsJsonAsync("/api/Institution", Dto(name: "Outra Instituição"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Contains("O email informado já está cadastrado.", body.Errors);
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ClearInstitutionsAsync();

        var response = await _api.Client.GetAsync("/api/Institution/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Instituição com id 9999 não encontrada.", body.Message);
    }

    [Fact]
    public async Task Put_ComInstituicaoInexistente_Retorna404()
    {
        await _api.ClearInstitutionsAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/Institution",
            new UpdateInstitutionDto(9999, "Instituição Feliz", "instituicao@email.com", "44999999999",
                "12345678987654", "foto.png", "senhaSegura123",
                "Instituição dedicada ao resgate de animais.", "08:00 às 18:00", "Rua das Flores, 123"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComInstituicaoInexistente_Retorna404()
    {
        await _api.ClearInstitutionsAsync();

        var response = await _api.Client.DeleteAsync("/api/Institution/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostaDoEndpoint_NaoExpoeSenha()
    {
        await _api.ClearInstitutionsAsync();
        var post = await _api.Client.PostAsJsonAsync("/api/Institution", Dto());
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var json = await _api.Client.GetStringAsync("/api/Institution");

        Assert.DoesNotContain("Password", json);
        Assert.DoesNotContain("senhaSegura123", json);
    }

    private sealed record ProblemBody(string Message, List<string> Errors);
}