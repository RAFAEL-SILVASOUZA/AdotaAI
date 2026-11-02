using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class AdmRepository(AdotaAIDbContext context) : IAdmRepository
{
    public async Task AddAsync(Adm adm, CancellationToken ct = default)
    {
        await context.Adms.AddAsync(adm, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Adm?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Adms.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Adm>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Adms
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(e => e.Email == email, ct);
    }

    public async Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default)
    {
        return await context.Employees.AnyAsync(e => e.Cpf == cpf, ct);
    }

    public async Task<bool> UpdateAsync(Adm adm, CancellationToken ct = default)
    {
        var tracked = await context.Adms.FindAsync([adm.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(adm);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Adms.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Adms.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}