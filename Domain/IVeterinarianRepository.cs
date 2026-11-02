namespace AdotaAI.Domain;

public interface IVeterinarianRepository
{
    Task AddAsync(Veterinarian veterinarian, CancellationToken ct = default);
    Task<Veterinarian?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Veterinarian>> GetAllAsync(CancellationToken ct = default);
    Task<bool> CrmvExistsAsync(string crmv, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default);
    Task<bool> UpdateAsync(Veterinarian veterinarian, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}