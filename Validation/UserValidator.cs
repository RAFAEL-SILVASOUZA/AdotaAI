using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("O nome do usuário não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome do usuário deve ter pelo menos 2 caracteres.")
            .MaximumLength(70).WithMessage("O nome do usuário não pode ter mais de 70 caracteres.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("O email do usuário não pode ser vazio.")
            .EmailAddress().WithMessage("O email do usuário não é válido.");

            RuleFor(user => user.Password)
            .NotEmpty().WithMessage("A senha do usuário não pode ser vazia.")
            .MinimumLength(6).WithMessage("A senha do usuário deve ter pelo menos 6 caracteres.")
            .MaximumLength(25).WithMessage("A senha do usuário não pode ter mais de 25 caracteres.");

            RuleFor(user => user.Phone)
            .NotEmpty().WithMessage("O telefone do usuário não pode ser vazio.")
            .MinimumLength(10).WithMessage("O telefone do usuário deve ter pelo menos 10 caracteres.")
            .MaximumLength(15).WithMessage("O telefone do usuário não pode ter mais de 15 caracteres.");

            RuleFor(user => user.Address)
            .NotEmpty().WithMessage("O endereço do usuário não pode ser vazio.")
            .MinimumLength(5).WithMessage("O endereço do usuário deve ter pelo menos 5 caracteres.")
            .MaximumLength(100).WithMessage("O endereço do usuário não pode ter mais de 100 caracteres.");

            RuleFor(user => user.Age)
            .InclusiveBetween(18, 100).WithMessage("A idade do usuário deve estar entre 18 e 100 anos.");

            RuleFor(user => user.Photo)
            .NotEmpty().WithMessage("A foto do usuário não pode ser vazia.");

            RuleFor(user => user.Gender)
            .IsInEnum().WithMessage("O gênero informado do usuário não é válido.");

            RuleFor(user => user.Id)
            .GreaterThan(0).WithMessage("O id do usuário deve ser maior que zero.");
    }
}