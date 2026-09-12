public class Pet
{

        public string Name { get; private set; } = string.Empty;

            public Species Species { get; private set; }

            public string Race { get; private set; } = string.Empty;
            
            public int Age { get; private set; } = 0;

            public bool IsFemale { get; private set; }
            
            public Size PetSize { get; private set; }

            public string Photo { get; private set; } = string.Empty;

            public string VetRecord { get; private set; } = string.Empty;
            
            public int Id { get; private set; }

            public string BehaviourDesc { get; private set; } = string.Empty;

            public Status PetStatus { get; private set; }

    public Pet(string name, Species species, string race, int age, bool isFemale, Size petSize, string photo, string vetRecord, int id, string behaviourDesc, Status petStatus)
    {
        Name = name;
        Species = species;
        Race = race;
        Age = age;
        IsFemale = isFemale;
        PetSize = petSize;
        Photo = photo;
        VetRecord = vetRecord;
        Id = id;
        BehaviourDesc = behaviourDesc;
        PetStatus = petStatus;
    }

}