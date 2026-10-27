namespace AdotaAI.Domain;

public interface IInstitutionRepository
{
    Task AddAsync(Institution institution, CancellationToken ct = default);
    Task<Institution?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Institution>> GetAllAsync(CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> UpdateAsync(Institution institution, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
