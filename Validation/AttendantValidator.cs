using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class AttendantValidator : AbstractValidator<Attendant>
{
    public AttendantValidator()
    {
        Include(new EmployeeValidator());

        RuleFor(attendant => attendant.InstitutionId)
            .GreaterThan(0).WithMessage("O id da instituição do atendente deve ser maior que zero.");
    }
}
