public class Institution
{

    public int Id  { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Photo { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Document { get; private set; } = string.Empty;
    public string OperatingHours { get; private set; } = string.Empty;
   
    public Institution(int id, string name, string email, string phone, string address, string photo, string password, string description, string document, string operatingHours)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        Photo = photo;
        Password = password;
        Description = description;
        Document = document;
        OperatingHours = operatingHours;
    } 

}