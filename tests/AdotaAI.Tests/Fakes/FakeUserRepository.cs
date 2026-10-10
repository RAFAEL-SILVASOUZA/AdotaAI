using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

// Fake em memória: permite testar o UserService sem banco.
// AddAsync emula o ValueGeneratedOnAdd do EF atribuindo o Id via reflection
// (as entidades têm private set, então o teste precisa imitar o comportamento
// do SaveChanges).
public class FakeUserRepository : IUserRepository
{
    private static readonly PropertyInfo IdProperty = typeof(User).GetProperty("Id")!;

    private readonly Dictionary<int, User> _users = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<User> Users => _users.Values;

    public Task AddAsync(User user, CancellationToken ct = default)
    {
        var nextId = _users.Count == 0 ? 1 : _users.Keys.Max() + 1;
        IdProperty.SetValue(user, nextId);
        _users[nextId] = user;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_users.TryGetValue(id, out var user) ? user : null);

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<User>>(_users.Values.OrderBy(u => u.Id).ToList());

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_users.Values.Any(u => u.Email == email));

    public Task<bool> UpdateAsync(User user, CancellationToken ct = default)
    {
        if (!_users.ContainsKey(user.Id))
            return Task.FromResult(false);

        _users[user.Id] = user;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_users.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}
