using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class InstitutionValidator : AbstractValidator<Institution>
{
    public InstitutionValidator()
    {
        RuleFor(institution => institution.Name)
            .NotEmpty().WithMessage("O nome da instituição não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome da instituição deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome da instituição não pode ter mais de 100 caracteres.");

        RuleFor(institution => institution.Email)
            .NotEmpty().WithMessage("O email da instituição não pode ser vazio.")
            .EmailAddress().WithMessage("O email da instituição não é válido.");

        RuleFor(institution => institution.Phone)
            .NotEmpty().WithMessage("O telefone da instituição não pode ser vazio.")
            .MinimumLength(10).WithMessage("O telefone da instituição deve ter pelo menos 10 caracteres.")
            .MaximumLength(15).WithMessage("O telefone da instituição não pode ter mais de 15 caracteres.");

        RuleFor(institution => institution.Address)
            .NotEmpty().WithMessage("O endereço da instituição não pode ser vazio.")
            .MinimumLength(5).WithMessage("O endereço da instituição deve ter pelo menos 5 caracteres.")
            .MaximumLength(100).WithMessage("O endereço da instituição não pode ter mais de 100 caracteres.");

        RuleFor(institution => institution.Password)
            .NotEmpty().WithMessage("A senha da instituição não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha da instituição deve ter pelo menos 6 caracteres.")
            .MaximumLength(25).WithMessage("A senha da instituição não pode ter mais de 25 caracteres.");

        RuleFor(institution => institution.Description)
            .NotEmpty().WithMessage("A descrição da instituição não pode ser vazia.")
            .MinimumLength(10).WithMessage("A descrição da instituição deve ter pelo menos 10 caracteres.")
            .MaximumLength(500).WithMessage("A descrição da instituição não pode ter mais de 500 caracteres.");

        RuleFor(institution => institution.Id)
            .GreaterThan(0).WithMessage("O id da instituição deve ser maior que zero.");
    }
}