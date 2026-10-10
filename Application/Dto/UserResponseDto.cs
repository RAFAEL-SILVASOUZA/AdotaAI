using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record UserResponseDto(
    int Id,
    string Name,
    string Email,
    string Phone,
    int Age,
    string Photo,
    Sex Gender,
    string Address)
{
    public static UserResponseDto FromEntity(User user) =>
        new(user.Id, user.Name, user.Email, user.Phone, user.Age, user.Photo, user.Gender, user.Address);
}
