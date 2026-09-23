public class Attendant : Employee
{

public int InstitutionId { get; private set; } = 0;

    public Attendant(int id, string name, string email, string password, string cpf, int institutionId) : base(id, name, email, password, cpf)
    {
        InstitutionId = institutionId;
    }
}
