using System.Reflection;
using AdotaAI.Domain;

namespace AdotaAI.Tests.Fakes;

public class FakeEmployeeRepository : IEmployeeRepository
{
    private static readonly PropertyInfo IdProperty =
        typeof(Employee).GetProperty("Id")!.DeclaringType!
            .GetProperty("Id", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

    private readonly Dictionary<int, Employee> _employees = new();

    public int SaveChangesCalls { get; private set; }

    public IReadOnlyCollection<Employee> Employees => _employees.Values;

    public Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        var nextId = _employees.Count == 0 ? 1 : _employees.Keys.Max() + 1;
        IdProperty.SetValue(employee, nextId);
        _employees[nextId] = employee;
        SaveChangesCalls++;
        return Task.CompletedTask;
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default) =>
        Task.FromResult(_employees.TryGetValue(id, out var employee) ? employee : null);

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Employee>>(_employees.Values.OrderBy(e => e.Id).ToList());

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_employees.Values.Any(e => e.Email == email));

    public Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default) =>
        Task.FromResult(_employees.Values.Any(e => e.Cpf == cpf));

    public Task<bool> UpdateAsync(Employee employee, CancellationToken ct = default)
    {
        if (!_employees.ContainsKey(employee.Id))
            return Task.FromResult(false);

        _employees[employee.Id] = employee;
        SaveChangesCalls++;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!_employees.Remove(id))
            return Task.FromResult(false);

        SaveChangesCalls++;
        return Task.FromResult(true);
    }
}