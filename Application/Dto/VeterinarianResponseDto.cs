using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record VeterinarianResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf,
    string Crmv,
    int InstitutionId
)
{
    public static VeterinarianResponseDto FromEntity(Veterinarian veterinarian) =>
        new(veterinarian.Id, veterinarian.Name, veterinarian.Email, veterinarian.Cpf, veterinarian.Crmv, veterinarian.InstitutionId);
}