using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record UpdateInstitutionDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Document,
    string Photo,
    string Password,
    string Description,
    string OperatingHours,
    string Address);