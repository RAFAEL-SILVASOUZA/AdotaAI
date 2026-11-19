using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakeVeterinarianRepository : IVeterinarianRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Veterinarian).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Veterinarian> _veterinarians = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Veterinarian> Veterinarians => _veterinarians.Values;

    public Task AddAsync(Veterinarian veterinarian, CancellationToken ct = default)
    {
        var nextId = _veterinarians.Count == 0 ? 1 : _veterinarians.Keys.Max() + 1;
        IdProperty.SetValue(veterinarian, nextId);
        _veterinarians[nextId] = veterinarian;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Veterinarian?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_veterinarians.TryGetValue(id, out var vet) ? vet : null);

    public Task<IReadOnlyList<Veterinarian>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Veterinarian>>(_veterinarians.Values.OrderBy(v => v.Id).ToList());

    public Task<bool> CrmvExistsAsync(string crmv, CancellationToken ct = default) =>
        Task.FromResult(_veterinarians.Values.Any(v => v.Crmv == crmv));

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_veterinarians.Values.Any(v => v.Email == email));

    public Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default) =>
        Task.FromResult(_veterinarians.Values.Any(v => v.Cpf == cpf));

    public Task<bool> UpdateAsync(Veterinarian veterinarian, CancellationToken ct = default)
    {
        if (!_veterinarians.ContainsKey(veterinarian.Id))
            return Task.FromResult(false);

        _veterinarians[veterinarian.Id] = veterinarian;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_veterinarians.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}