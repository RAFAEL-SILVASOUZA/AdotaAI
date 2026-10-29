using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record UpdatePetDto(
    string Name,
    Species Species,
    string Race,
    int Age,
    bool IsFemale,
    Size PetSize,
    string Photo,
    string VetRecord,
    string BehaviourDesc,
    Status PetStatus);
