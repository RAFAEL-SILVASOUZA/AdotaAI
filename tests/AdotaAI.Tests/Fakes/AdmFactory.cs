using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class AdmFactory
{
    public static Adm Valid(int id = 1) =>
        new(
            id: id,
            name: "Fernanda Lima",
            email: "fernanda.adm@email.com",
            password: "senhaSegura123",
            cpf: "12345678901");
}