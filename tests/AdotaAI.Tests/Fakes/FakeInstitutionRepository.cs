using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakeInstitutionRepository : IInstitutionRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Institution).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Institution> _institutions = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Institution> Institutions => _institutions.Values;

    public Task AddAsync(Institution institution, CancellationToken ct = default)
    {
        var nextId = _institutions.Count == 0 ? 1 : _institutions.Keys.Max() + 1;
        IdProperty.SetValue(institution, nextId);
        _institutions[nextId] = institution;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Institution?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_institutions.TryGetValue(id, out var institution) ? institution : null);

    public Task<IReadOnlyList<Institution>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Institution>>(_institutions.Values.OrderBy(i => i.Id).ToList());

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_institutions.Values.Any(i => i.Email == email));

    public Task<bool> UpdateAsync(Institution institution, CancellationToken ct = default)
    {
        if (!_institutions.ContainsKey(institution.Id))
            return Task.FromResult(false);

        _institutions[institution.Id] = institution;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_institutions.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}