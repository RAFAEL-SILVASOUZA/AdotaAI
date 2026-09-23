namespace AdotaAI.Domain;

public class Veterinarian(int id, string name, string email, string password, string cpf, string crmv, int institutionId) : Employee(id, name, email, password, cpf)
{
    public string Crmv { get; private set; } = crmv;
    public int InstitutionId { get; private set; } = institutionId;
}