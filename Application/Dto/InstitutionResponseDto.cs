using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record InstitutionResponseDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    string Document,
    string Photo,
    string Description,
    string OperatingHours,
    string Address)
{
    public static InstitutionResponseDto FromEntity(Institution institution) =>
        new(institution.Id, institution.Name, institution.Email, institution.Phone, institution.Document, institution.Photo, institution.Description, institution.OperatingHours, institution.Address);
}
