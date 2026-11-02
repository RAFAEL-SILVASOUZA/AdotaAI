namespace AdotaAI.Domain;

public interface IEmployeeRepository
{
    Task AddAsync(Employee employee, CancellationToken ct = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default);
    Task<bool> UpdateAsync(Employee employee, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}