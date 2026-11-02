namespace AdotaAI.Application.Dto;

public record CreateEmployeeDto(
    string Name,
    string Email,
    string Password,
    string Cpf
);