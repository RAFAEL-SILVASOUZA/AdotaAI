using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class VeterinarianValidator : AbstractValidator<Veterinarian>
{
    
    public VeterinarianValidator()
    {
        Include (new EmployeeValidator());

        RuleFor(veterinarian => veterinarian.Crmv)
            .NotEmpty().WithMessage("O CRMV do veterinário não pode ser vazio.")
            .MinimumLength(5).WithMessage("O CRMV do veterinário deve ter pelo menos 5 caracteres.")
            .MaximumLength(10).WithMessage("O CRMV do veterinário não pode ter mais de 10 caracteres.");

        RuleFor(veterinarian => veterinarian.InstitutionId)
            .GreaterThan(0).WithMessage("O id do veterinário deve ser maior que zero.");
    }
}