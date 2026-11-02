namespace AdotaAI.Domain;

public interface IAdmRepository
{
    Task AddAsync(Adm adm, CancellationToken ct = default);
    Task<Adm?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Adm>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default);
    Task<bool> UpdateAsync(Adm adm, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}