namespace AdotaAI.Domain;

public interface IPetRepository
{
    Task AddAsync(Pet pet, CancellationToken ct = default);
    Task<Pet?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Pet>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Pet>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default);
    Task<bool> UpdateAsync(Pet pet, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
