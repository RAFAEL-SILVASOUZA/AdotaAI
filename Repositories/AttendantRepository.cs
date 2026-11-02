using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class AttendantRepository(AdotaAIDbContext context) : IAttendantRepository
{
    public async Task AddAsync(Attendant attendant, CancellationToken ct = default)
    {
        await context.Attendants.AddAsync(attendant, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Attendant?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Attendants.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Attendant>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Attendants
            .AsNoTracking()
            .OrderBy(a => a.Id)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Attendant>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        return await context.Attendants
            .AsNoTracking()
            .Where(a => a.InstitutionId == institutionId)
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

    public async Task<bool> UpdateAsync(Attendant attendant, CancellationToken ct = default)
    {
        var tracked = await context.Attendants.FindAsync([attendant.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(attendant);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Attendants.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Attendants.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}