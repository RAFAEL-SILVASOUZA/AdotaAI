namespace AdotaAI.Domain;

public interface IAttendantRepository
{
    Task AddAsync(Attendant attendant, CancellationToken ct = default);
    Task<Attendant?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Attendant>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Attendant>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<bool> CpfExistsAsync(string cpf, CancellationToken ct = default);
    Task<bool> UpdateAsync(Attendant attendant, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}