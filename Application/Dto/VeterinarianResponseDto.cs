namespace AdotaAI.Application.Dto;

public record VeterinarianResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf,
    string Crmv,
    int InstitutionId
);