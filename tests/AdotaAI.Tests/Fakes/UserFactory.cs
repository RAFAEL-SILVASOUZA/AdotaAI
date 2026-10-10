using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public static class UserFactory
{
    // Um usuário válido por padrão; cada teste altera apenas o que quer verificar.
    public static User Valid(int id = 1) =>
        new(
            id: id,
            name: "Maria Silva",
            email: "maria@email.com",
            phone: "11999998888",
            age: 25,
            photo: "perfil.jpg",
            password: "senhaSegura123",
            gender: Sex.Female,
            address: "Rua das Flores, 123");
}
