namespace AdotaAI.Application.Dto;

public record CreateAttendantDto(
    string Name,
    string Email,
    string Password,
    string Cpf,
    int InstitutionId
);