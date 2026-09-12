public class Veterinarian : Employee
{

    public string Crmv { get; private set; } = string.Empty;
    public int InstitutionId { get; private set; } = 0;

    public Veterinarian(int id, string name, string email, string password, string cpf, string crmv, int institutionId) : base(id, name, email, password, cpf)
    {
        Crmv = crmv;
        InstitutionId = institutionId;
    }
    
}