namespace AdotaAI.Domain;

public class Pet(string name, Species species, string race, int age, bool isFemale, Size petSize, string photo, string vetRecord, int id, string behaviourDesc, Status petStatus, int institutionId)
{
    public string Name { get; private set; } = name;
    public Species Species { get; private set; } = species;
    public string Race { get; private set; } = race;
    public int Age { get; private set; } = age;
    public bool IsFemale { get; private set; } = isFemale;
    public Size PetSize { get; private set; } = petSize;
    public string Photo { get; private set; } = photo;
    public string VetRecord { get; private set; } = vetRecord;
    public int Id { get; private set; } = id;
    public string BehaviourDesc { get; private set; } = behaviourDesc;
    public Status PetStatus { get; private set; } = petStatus;
    public int InstitutionId { get; private set; } =  institutionId;
}