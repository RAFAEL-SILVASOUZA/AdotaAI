using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class VeterinarianRepository(AdotaAIDbContext context) : IVeterinarianRepository
{
    public async Task AddAsync(Veterinarian veterinarian, CancellationToken ct = default)
    {
        await context.Veterinarians.AddAsync(veterinarian, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Veterinarian?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Veterinarians.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Veterinarian>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Veterinarians
            .AsNoTracking()
            .OrderBy(v => v.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> CrmvExistsAsync(string crmv, CancellationToken ct = default)
    {
        return await context.Veterinarians.AnyAsync(v => v.Crmv == crmv, ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(e => e.Email == email, ct);
    }

    public async Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(e => e.Cpf == cpf, ct);
    }

    public async Task<bool> UpdateAsync(Veterinarian veterinarian, CancellationToken ct = default)
    {
        var tracked = await context.Veterinarians.FindAsync([veterinarian.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(veterinarian);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Veterinarians.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Veterinarians.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}