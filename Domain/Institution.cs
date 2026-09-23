namespace AdotaAI.Domain;

public class Institution(int id, string name, string email, string phone, string address, string photo, string password, string description, string document, string operatingHours)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string Email { get; private set; } = email;
    public string Phone { get; private set; } = phone;
    public string Address { get; private set; } = address;
    public string Photo { get; private set; } = photo;
    public string Password { get; private set; } = password;
    public string Description { get; private set; } = description;
    public string Document { get; private set; } = document;
    public string OperatingHours { get; private set; } = operatingHours;
}