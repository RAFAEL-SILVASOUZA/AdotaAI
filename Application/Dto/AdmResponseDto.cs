using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record AdmResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf
)
{
    public static AdmResponseDto FromEntity(Adm adm) =>
        new(adm.Id, adm.Name, adm.Email, adm.Cpf);
}