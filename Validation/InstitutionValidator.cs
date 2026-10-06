using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class InstitutionValidator : AbstractValidator<Institution>
{
    public InstitutionValidator()
    {
        RuleFor(institution => institution.Id)
            .GreaterThan(0).WithMessage("O id da instituição deve ser maior que zero.");

        RuleFor(institution => institution.Name)
            .NotEmpty().WithMessage("O nome da instituição não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome da instituição deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome da instituição não pode ter mais de 100 caracteres.");

        RuleFor(institution => institution.Email)
            .NotEmpty().WithMessage("O e-mail da instituição não pode ser vazio.")
            .EmailAddress().WithMessage("O e-mail da instituição deve ser um e-mail válido.");

        RuleFor(institution => institution.Phone)
            .NotEmpty().WithMessage("O telefone da instituição não pode ser vazio.")
            .MinimumLength(10).WithMessage("O telefone da instituição deve ter pelo menos 10 caracteres.")
            .MaximumLength(15).WithMessage("O telefone da instituição não pode ter mais de 15 caracteres.");

        RuleFor(institution => institution.Address)
            .NotEmpty().WithMessage("O endereço da instituição não pode ser vazio.");

        RuleFor(institution => institution.Photo)
            .NotEmpty().WithMessage("A foto da instituição não pode ser vazia.");

        RuleFor(institution => institution.Password)
            .NotEmpty().WithMessage("A senha da instituição não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha da instituição deve ter pelo menos 6 caracteres.");

        RuleFor(institution => institution.Description)
            .NotEmpty().WithMessage("A descrição da instituição não pode ser vazia.")
            .MaximumLength(500).WithMessage("A descrição da instituição não pode ter mais de 500 caracteres.");

        RuleFor(institution => institution.Document)
            .NotEmpty().WithMessage("O documento da instituição não pode ser vazio.");

        RuleFor(institution => institution.OperatingHours)
            .NotEmpty().WithMessage("O horário de funcionamento da instituição não pode ser vazio.");
    }
}
