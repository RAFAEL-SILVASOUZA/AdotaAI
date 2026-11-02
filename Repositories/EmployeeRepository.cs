using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class EmployeeRepository(AdotaAIDbContext context) : IEmployeeRepository
{
    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        await context.Employees.AddAsync(employee, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Employees.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(employee => employee.Email == email, ct);
    }

    public async Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(employee => employee.Cpf == cpf, ct);
    }

    public async Task<bool> UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        var tracked = await context.Employees.FindAsync([employee.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(employee);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Employees.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Employees.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}