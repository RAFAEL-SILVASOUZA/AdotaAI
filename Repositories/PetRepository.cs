using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class PetRepository(AdotaAIDbContext context) : IPetRepository
{
    public async Task AddAsync(Pet pet, CancellationToken ct = default)
    {
        await context.Pets.AddAsync(pet, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<Pet?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Pets.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Pets.AsNoTracking().OrderBy(pet => pet.Id).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Pet>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        return await context.Pets
            .AsNoTracking()
            .Where(pet => pet.InstitutionId == institutionId)
            .OrderBy(pet => pet.Id)
            .ToListAsync(ct);
    }

    public async Task<bool> UpdateAsync(Pet pet, CancellationToken ct = default)
    {
        var tracked = await context.Pets.FindAsync([pet.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(pet);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Pets.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Pets.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}