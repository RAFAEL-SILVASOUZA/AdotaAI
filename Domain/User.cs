public class User
{

    public string Name  { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public int Age { get; private set; } = 0;
    public int Id { get; private set; } = 0;
    public string Photo { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public Sex Gender { get; private set; }
    public string Address { get; private set; } = string.Empty;

    public User(int id, string name, string email, string phone, int age, string photo, string password, Sex gender, string address)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        Age = age;
        Photo = photo;
        Password = password;
        Gender = gender;
        Address = address;
    }

}