using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class EmployeeValidator : AbstractValidator<Employee>
{

   public EmployeeValidator()
    {
        RuleFor(employee => employee.Name)
            .NotEmpty().WithMessage("O nome do funcionário não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome do funcionário deve ter pelo menos 2 caracteres.")
            .MaximumLength(70).WithMessage("O nome do funcionário não pode ter mais de 70 caracteres.");

        RuleFor(employee => employee.Email)
            .NotEmpty().WithMessage("O email do funcionário não pode ser vazio.")
            .EmailAddress().WithMessage("O email do funcionário não é válido.");

        RuleFor(employee => employee.Password)
            .NotEmpty().WithMessage("A senha do funcionário não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha do funcionário deve ter pelo menos 6 caracteres.")
            .MaximumLength(25).WithMessage("A senha do funcionário não pode ter mais de 25 caracteres.");

        RuleFor(employee => employee.CPF)
            .NotEmpty().WithMessage("O CPF do funcionário não pode ser vazio.")
            .Length(11).WithMessage("O CPF do funcionário deve ter exatamente 11 caracteres.");

        RuleFor(employee => employee.Id)
            .GreaterThan(0).WithMessage("O id do funcionário deve ser maior que zero.");
        
    } 
}
