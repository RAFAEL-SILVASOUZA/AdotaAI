using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class VeterinarianFactory
{
    public static Veterinarian Valid(int id = 1, int institutionId = 1) =>
        new(
            id: id,
            name: "Dra. Ana Paula",
            email: "ana.vet@email.com",
            password: "senhaSegura123",
            cpf: "12345678901",
            crmv: "12345-SP",
            institutionId: institutionId);
}