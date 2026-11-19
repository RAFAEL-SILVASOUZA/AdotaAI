using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakeAttendantRepository : IAttendantRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Attendant).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Attendant> _attendants = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Attendant> Attendants => _attendants.Values;

    public Task AddAsync(Attendant attendant, CancellationToken ct = default)
    {
        var nextId = _attendants.Count == 0 ? 1 : _attendants.Keys.Max() + 1;
        IdProperty.SetValue(attendant, nextId);
        _attendants[nextId] = attendant;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Attendant?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_attendants.TryGetValue(id, out var attendant) ? attendant : null);

    public Task<IReadOnlyList<Attendant>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Attendant>>(_attendants.Values.OrderBy(a => a.Id).ToList());

    public Task<IReadOnlyList<Attendant>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Attendant>>(
            _attendants.Values.Where(a => a.InstitutionId == institutionId).OrderBy(a => a.Id).ToList());

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_attendants.Values.Any(a => a.Email == email));

    public Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default) =>
        Task.FromResult(_attendants.Values.Any(a => a.Cpf == cpf));

    public Task<bool> UpdateAsync(Attendant attendant, CancellationToken ct = default)
    {
        if (!_attendants.ContainsKey(attendant.Id))
            return Task.FromResult(false);

        _attendants[attendant.Id] = attendant;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_attendants.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}