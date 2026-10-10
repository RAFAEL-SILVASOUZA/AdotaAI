using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record CreateUserDto(
    string Name,
    string Email,
    string Phone,
    int Age,
    string Photo,
    string Password,
    Sex Gender,
    string Address);
