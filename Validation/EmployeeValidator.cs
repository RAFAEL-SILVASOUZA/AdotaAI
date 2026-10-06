using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(employee => employee.Id)
            .GreaterThan(0).WithMessage("O id do funcionário deve ser maior que zero.");

        RuleFor(employee => employee.Name)
            .NotEmpty().WithMessage("O nome do funcionário não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome do funcionário deve ter pelo menos 2 caracteres.")
            .MaximumLength(100).WithMessage("O nome do funcionário não pode ter mais de 100 caracteres.");

        RuleFor(employee => employee.Email)
            .NotEmpty().WithMessage("O e-mail do funcionário não pode ser vazio.")
            .EmailAddress().WithMessage("O e-mail do funcionário deve ser um e-mail válido.");

        RuleFor(employee => employee.Password)
            .NotEmpty().WithMessage("A senha do funcionário não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha do funcionário deve ter pelo menos 6 caracteres.");

        RuleFor(employee => employee.CPF)
            .NotEmpty().WithMessage("O CPF do funcionário não pode ser vazio.")
            .Length(11).WithMessage("O CPF do funcionário deve ter 11 dígitos.")
            .Matches(@"^\d{11}$").WithMessage("O CPF do funcionário deve conter apenas números.");
    }
}
