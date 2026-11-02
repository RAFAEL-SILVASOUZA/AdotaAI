namespace AdotaAI.Application.Dto;

public record CreateAdmDto(
    string Name,
    string Email,
    string Password,
    string Cpf
);