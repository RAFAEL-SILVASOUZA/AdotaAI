public class Employee
{

    public int Id { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;

    public Employee(int id, string name, string email, string password, string cpf)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password;
        CPF = cpf;
    }

}