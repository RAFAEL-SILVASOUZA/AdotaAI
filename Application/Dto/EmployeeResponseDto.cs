using AdotaAI.Domain;

namespace AdotaAI.Application.Dto;

public record EmployeeResponseDto(
    int Id,
    string Name,
    string Email,
    string Cpf
)
{
    public static EmployeeResponseDto FromEntity(Employee employee) =>
        new(employee.Id, employee.Name, employee.Email, employee.Cpf);
}