using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AdotaAI.Application.Dto;
using AdotaAI.Domain;
using Xunit;

namespace AdotaAI.Tests.Integration;

public class PetControllerTests : IClassFixture<ApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ApiFactory _api;

    public PetControllerTests(ApiFactory api)
    {
        _api = api;
    }

    private static CreatePetDto Dto(
        int institutionId,
        string name = "Rex",
        Species species = Species.Dog,
        string race = "Vira-lata",
        string photo = "rex.jpg",
        int age = 3,
        bool isFemale = false,
        Size petSize = Size.Medium,
        string behaviourDesc = "Dócil e brincalhão.",
        string vetRecord = "Vacinas em dia.",
        Status petStatus = Status.Available) =>
        new(name, species, race, photo, age, isFemale, petSize, behaviourDesc, vetRecord, institutionId, petStatus);

    [Fact]
    public async Task Post_ComDadosValidos_Retorna201EPetPersistido()
    {
        await _api.ClearPetsAndInstitutionsAsync();
        var institutionId = await _api.SeedInstitutionAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Pet", Dto(institutionId));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var id = await response.Content.ReadFromJsonAsync<int>(JsonOptions);
        Assert.True(id > 0);

        var created = await _api.Client.GetFromJsonAsync<PetResponseDto>($"/api/Pet/{id}", JsonOptions);
        Assert.NotNull(created);
        Assert.Equal(id, created.Id);
        Assert.Equal("Rex", created.Name);
    }

    [Fact]
    public async Task Post_ComCamposObrigatoriosInvalidos_Retorna400ComMensagensCertas()
    {
        await _api.ClearPetsAndInstitutionsAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Pet",
            Dto(institutionId: 0, name: "", race: "", photo: "", age: -1, behaviourDesc: "", vetRecord: ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.DoesNotContain("O id do pet deve ser maior que zero.", body.Errors);
        Assert.Contains("O nome do pet não pode ser vazio.", body.Errors);
        Assert.Contains("A raça do pet não pode ser vazia.", body.Errors);
        Assert.Contains("A idade do pet deve estar entre 0 e 30 anos.", body.Errors);
        Assert.Contains("A foto do pet não pode ser vazia.", body.Errors);
        Assert.Contains("O prontuário veterinário não pode ser vazio.", body.Errors);
        Assert.Contains("A descrição de comportamento não pode ser vazia.", body.Errors);
        Assert.Contains("O id da instituição deve ser maior que zero.", body.Errors);
    }

    [Fact]
    public async Task Post_ComInstitutionIdInexistente_Retorna400()
    {
        await _api.ClearPetsAndInstitutionsAsync();

        var response = await _api.Client.PostAsJsonAsync("/api/Pet", Dto(institutionId: 9999));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Contains("Instituição informada não encontrada.", body.Errors);
    }

    [Fact]
    public async Task GetPorId_Inexistente_Retorna404ComMensagem()
    {
        await _api.ClearPetsAndInstitutionsAsync();

        var response = await _api.Client.GetAsync("/api/Pet/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal("Pet com id 9999 não encontrado.", body.Message);
    }

    [Fact]
    public async Task Put_ComPetInexistente_Retorna404()
    {
        await _api.ClearPetsAndInstitutionsAsync();

        var response = await _api.Client.PutAsJsonAsync("/api/Pet/9999",
            new UpdatePetDto("Rex", Species.Dog, "Vira-lata", 3, false,
                Size.Medium, "rex.jpg", "Vacinas em dia.", "Dócil.", Status.Available));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ComPetInexistente_Retorna404()
    {
        await _api.ClearPetsAndInstitutionsAsync();

        var response = await _api.Client.DeleteAsync("/api/Pet/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record ProblemBody(string Message, List<string> Errors);
}