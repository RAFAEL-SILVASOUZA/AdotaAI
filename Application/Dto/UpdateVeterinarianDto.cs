namespace AdotaAI.Application.Dto;

public record UpdateVeterinarianDto(
    string Name,
    string Email,
    string Password,
    string Cpf,
    string Crmv,
    int InstitutionId
);