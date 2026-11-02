namespace AdotaAI.Application.Dto;

public record AttendantResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf,
    int InstitutionId
);