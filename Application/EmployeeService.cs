using AdotaAI.Application.Dto;
using AdotaAI.Domain;

namespace AdotaAI.Application;

public class EmployeeService(IEmployeeRepository employeeRepository)
{
    public async Task<EmployeeResponseDto> AddAsync(CreateEmployeeDto dto, CancellationToken ct = default)
    {
        if (await employeeRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await employeeRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        var employee = new Employee(0, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await employeeRepository.AddAsync(employee, ct);

        return new EmployeeResponseDto(employee.Id, employee.Name, employee.Email, employee.Cpf);
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await employeeRepository.GetByIdAsync(id, ct);
        return employee is null ? null : new EmployeeResponseDto(employee.Id, employee.Name, employee.Email, employee.Cpf);
    }

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var employees = await employeeRepository.GetAllAsync(ct);
        return employees
            .Select(e => new EmployeeResponseDto(e.Id, e.Name, e.Email, e.Cpf))
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateEmployeeDto dto, CancellationToken ct = default)
    {
        var existing = await employeeRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        if (existing.Email != dto.Email && await employeeRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await employeeRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        var updatedEmployee = new Employee(id, dto.Name, dto.Email, dto.Password, dto.Cpf);
        return await employeeRepository.UpdateAsync(updatedEmployee, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await employeeRepository.DeleteAsync(id, ct);
    }
}