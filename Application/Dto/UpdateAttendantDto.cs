namespace AdotaAI.Application.Dto;

public record UpdateAttendantDto(
    string Name,
    string Email,
    string Password,
    string Cpf,
    int InstitutionId
);