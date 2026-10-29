using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record CreatePetDto(
    string Name,
    Species Species,
    string Race,
    string Photo,
    int Age,
    bool IsFemale,
    Size PetSize,
    string BehaviourDesc,
    string VetRecord,
    int InstitutionId,
    Status PetStatus);
