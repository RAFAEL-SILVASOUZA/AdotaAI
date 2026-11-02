namespace AdotaAI.Application.Dto;

public record UpdateEmployeeDto(
    string Name,
    string Email,
    string Password,
    string Cpf
);