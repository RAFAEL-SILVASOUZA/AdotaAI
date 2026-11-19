using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakeAdmRepository : IAdmRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Adm).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Adm> _adms = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Adm> Adms => _adms.Values;

    public Task AddAsync(Adm adm, CancellationToken ct = default)
    {
        var nextId = _adms.Count == 0 ? 1 : _adms.Keys.Max() + 1;
        IdProperty.SetValue(adm, nextId);
        _adms[nextId] = adm;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Adm?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_adms.TryGetValue(id, out var adm) ? adm : null);

    public Task<IReadOnlyList<Adm>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Adm>>(_adms.Values.OrderBy(a => a.Id).ToList());

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_adms.Values.Any(a => a.Email == email));

    public Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default) =>
        Task.FromResult(_adms.Values.Any(a => a.Cpf == cpf));

    public Task<bool> UpdateAsync(Adm adm, CancellationToken ct = default)
    {
        if (!_adms.ContainsKey(adm.Id))
            return Task.FromResult(false);

        _adms[adm.Id] = adm;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_adms.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}