namespace AdotaAI.Domain;

public class Attendant(int id, string name, string email, string password, string cpf, int institutionId) : Employee(id, name, email, password, cpf)
{
    public int InstitutionId { get; private set; } = institutionId;
}
