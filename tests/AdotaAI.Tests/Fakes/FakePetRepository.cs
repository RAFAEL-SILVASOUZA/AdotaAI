using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakePetRepository : IPetRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Pet).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Pet> _pets = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Pet> Pets => _pets.Values;

    public Task AddAsync(Pet pet, CancellationToken ct = default)
    {
        var nextId = _pets.Count == 0 ? 1 : _pets.Keys.Max() + 1;
        IdProperty.SetValue(pet, nextId);
        _pets[nextId] = pet;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Pet?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_pets.TryGetValue(id, out var pet) ? pet : null);

    public Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Pet>>(_pets.Values.OrderBy(p => p.Id).ToList());

    public Task<IReadOnlyList<Pet>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Pet>>(
            _pets.Values.Where(p => p.InstitutionId == institutionId).OrderBy(p => p.Id).ToList());

    public Task<bool> UpdateAsync(Pet pet, CancellationToken ct = default)
    {
        if (!_pets.ContainsKey(pet.Id))
            return Task.FromResult(false);

        _pets[pet.Id] = pet;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_pets.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}