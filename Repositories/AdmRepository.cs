using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;

namespace AdotaAI.Repositories;

public class AdmRepository(AdotaAIDbContext context)
    : EmployeeRepositoryBase<Adm>(context), IAdmRepository;