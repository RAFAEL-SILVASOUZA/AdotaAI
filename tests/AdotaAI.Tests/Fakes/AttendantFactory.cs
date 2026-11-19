using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class AttendantFactory
{
    public static Attendant Valid(int id = 1, int institutionId = 1) =>
        new(
            id: id,
            name: "Mariana Costa",
            email: "mariana.atend@email.com",
            password: "senhaSegura123",
            cpf: "12345678901",
            institutionId: institutionId);
}