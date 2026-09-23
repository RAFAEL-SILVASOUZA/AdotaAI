namespace AdotaAI.Domain;

public class User(int id, string name, string email, string phone, int age, string photo, string password, Sex gender, string address)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Phone { get; private set; } = phone;
    public int Age { get; private set; } = age;
    public string Photo { get; private set; } = photo;
    public string Password { get; private set; } = password;
    public Sex Gender { get; private set; } = gender;
    public string Address { get; private set; } = address;
}