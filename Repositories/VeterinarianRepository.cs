using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class VeterinarianRepository(AdotaAIDbContext context)
    : EmployeeRepositoryBase<Veterinarian>(context), IVeterinarianRepository
{
    public async Task<bool> CrmvExistsAsync(string crmv, CancellationToken ct = default)
    {
        return await Set.AnyAsync(v => v.Crmv == crmv, ct);
    }
}