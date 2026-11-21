using AdotaAI.Domain;
using AdotaAI.Infrastructure.Data;

namespace AdotaAI.Repositories;

public class EmployeeRepository(AdotaAIDbContext context)
    : EmployeeRepositoryBase<Employee>(context), IEmployeeRepository;