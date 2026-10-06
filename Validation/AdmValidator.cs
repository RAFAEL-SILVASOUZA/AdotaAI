using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class AdmValidator : AbstractValidator<Adm>
{
    public AdmValidator()
    {
        Include(new EmployeeValidator());
    }
}
