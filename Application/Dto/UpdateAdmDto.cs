namespace AdotaAI.Application.Dto;

public record UpdateAdmDto(
    string Name,
    string Email,
    string Password,
    string Cpf
);