namespace AdotaAI.Domain;

public class Employee(int id, string name, string email, string password, string cpf)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Password { get; private set; } = password;
    public string CPF { get; private set; } = cpf;
}