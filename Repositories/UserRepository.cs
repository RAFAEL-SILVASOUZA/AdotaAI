using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AdotaAI.Repositories;

public class UserRepository(AdotaAIDbContext context) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Users.FindAsync([id], ct);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Users.AsNoTracking().OrderBy(user => user.Id).ToListAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        return await context.Users.AnyAsync(user => user.Email == email, ct);
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct = default)
    {
        var tracked = await context.Users.FindAsync([user.Id], ct);
        if (tracked is null)
            return false;

        context.Entry(tracked).CurrentValues.SetValues(user);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var tracked = await context.Users.FindAsync([id], ct);
        if (tracked is null)
            return false;

        context.Users.Remove(tracked);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
