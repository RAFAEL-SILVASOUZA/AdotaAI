namespace AdotaAI.Application.Dto;

public record CreateVeterinarianDto(
    string Name,
    string Email,
    string Password,
    string Cpf,
    string Crmv,
    int InstitutionId
);