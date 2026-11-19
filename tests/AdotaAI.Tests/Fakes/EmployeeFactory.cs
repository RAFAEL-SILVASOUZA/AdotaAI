using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class EmployeeFactory
{
    public static Employee Valid(int id = 1) =>
        new(
            id: id,
            name: "Carlos Souza",
            email: "carlos.func@email.com",
            password: "senhaSegura123",
            cpf: "12345678901");
}