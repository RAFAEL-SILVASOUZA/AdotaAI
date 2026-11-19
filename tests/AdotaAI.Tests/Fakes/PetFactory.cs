using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class PetFactory
{
    public static Pet Valid(int id = 1, int institutionId = 1) =>
        new(
            id: id,
            name: "Rex",
            species: Species.Dog,
            race: "Vira-lata",
            age: 3,
            isFemale: false,
            petSize: Size.Medium,
            photo: "rex.jpg",
            vetRecord: "Vacinas em dia e castrado.",
            behaviourDesc: "Dócil, brincalhão e se dá bem com crianças.",
            petStatus: Status.Available,
            institutionId: institutionId);
}