using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class InstitutionFactory
{
    public static Institution Valid(int id = 1) =>
        new(
            id: id,
            name: "Instituição Feliz",
            email: "instituicao@email.com",
            phone: "44999999999",
            document: "12345678987654",
            photo: "foto.png",
            password: "senhaSegura123",
            description: "Instituição dedicada ao resgate de animais.",
            operatingHours: "08:00 às 18:00",
            address: "Rua das Flores, 123");
}