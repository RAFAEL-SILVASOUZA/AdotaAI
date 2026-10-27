using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class InstitutionRepository(AdotaAIDbContext context) : IInstitutionRepository
{
    public async Task AddAsync(Institution institution, CancellationToken ct = default)
    {
        await context.Institutions.AddAsync(institution, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Institution?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Institutions.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Institution>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Institutions.AsNoTracking().OrderBy(institution => institution.Id).ToListAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await context.Institutions.AnyAsync(institution => institution.Email == email, ct);
    }

    public async Task<bool> UpdateAsync(Institution institution, CancellationToken ct = default)
    {
        var tracked = await context.Institutions.FindAsync([institution.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(institution);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Institutions.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Institutions.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
