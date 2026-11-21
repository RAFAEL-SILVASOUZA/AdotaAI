using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record AttendantResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf,
    int InstitutionId
)
{
    public static AttendantResponseDto FromEntity(Attendant attendant) =>
        new(attendant.Id, attendant.Name, attendant.Email, attendant.Cpf, attendant.InstitutionId);
}