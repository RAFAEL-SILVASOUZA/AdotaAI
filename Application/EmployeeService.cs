using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class EmployeeService(IEmployeeRepository employeeRepository, IValidator<Employee> validator)
{
    public async Task<EmployeeResponseDto> AddAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        var employee = new Employee(0, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await ValidateAsync(employee, ct);

        if (await employeeRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await employeeRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        await employeeRepository.AddAsync(employee, ct);

        return EmployeeResponseDto.FromEntity(employee);
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, ct);
        return employee is null ? null : EmployeeResponseDto.FromEntity(employee);
    }

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var employees = await employeeRepository.GetAllAsync(ct);
        return employees
            .Select(EmployeeResponseDto.FromEntity)
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var existing = await employeeRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        var updatedEmployee = new Employee(id, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await ValidateAsync(updatedEmployee, ct);

        if (existing.Email != dto.Email && await employeeRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await employeeRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        return await employeeRepository.UpdateAsync(updatedEmployee, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await employeeRepository.DeleteAsync(id, ct);
    }

    private async Task ValidateAsync(Employee employee, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(employee, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}