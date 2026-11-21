using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public abstract class EmployeeRepositoryBase<TEntity>(AdotaAIDbContext context)
    where TEntity : Employee
{
    protected AdotaAIDbContext Context => context;
    protected DbSet<TEntity> Set => context.Set<TEntity>();

    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        await Set.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await Set.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await Set
            .AsNoTracking()
            .OrderBy(e => e.Id)
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

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default)
    {
        var tracked = await Set.FindAsync([entity.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(entity);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await Set.FindAsync([id], ct);
        if (tracked is null)
            return false;

        Set.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}