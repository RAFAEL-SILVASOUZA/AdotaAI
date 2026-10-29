using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record PetResponseDto(
    int Id,
    string Name,
    Species Species,
    string Race,
    int Age,
    bool IsFemale,
    Size PetSize,
    string Photo,
    string VetRecord,
    string BehaviourDesc,
    Status PetStatus,
    int InstitutionId)
{
    public static PetResponseDto FromEntity(Pet pet) =>
        new(pet.Id, pet.Name, pet.Species, pet.Race, pet.Age, pet.IsFemale, pet.PetSize, pet.Photo, pet.VetRecord, pet.BehaviourDesc, pet.PetStatus, pet.InstitutionId);
}
