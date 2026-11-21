using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class AttendantRepository(AdotaAIDbContext context)
    : EmployeeRepositoryBase<Attendant>(context), IAttendantRepository
{
    public async Task<IReadOnlyList<Attendant>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .Where(a => a.InstitutionId == institutionId)
            .ToListAsync(ct);
    }
}