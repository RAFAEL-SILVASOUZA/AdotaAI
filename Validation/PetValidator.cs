using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Validation;

public class PetValidator : AbstractValidator<Pet>
{
    public PetValidator()
    {
        RuleFor(pet => pet.Name)
            .NotEmpty().WithMessage("O nome do pet não pode ser vazio.")
            .MinimumLength(2).WithMessage("O nome do pet deve ter pelo menos 2 caracteres.")
            .MaximumLength(50).WithMessage("O nome do pet não pode ter mais de 50 caracteres.");

        RuleFor(pet => pet.Race)
            .NotEmpty().WithMessage("A raça do pet não pode ser vazia.")
            .MinimumLength(2).WithMessage("A raça do pet deve ter pelo menos 2 caracteres.")
            .MaximumLength(50).WithMessage("A raça do pet não pode ter mais de 50 caracteres.");

        RuleFor(pet => pet.Age)
            .InclusiveBetween(0, 30).WithMessage("A idade do pet deve estar entre 0 e 30 anos.");

        RuleFor(pet => pet.Photo)
            .NotEmpty().WithMessage("A foto do pet não pode ser vazia.");

        RuleFor(pet => pet.VetRecord)
            .NotEmpty().WithMessage("O prontuário veterinário não pode ser vazio.");

        RuleFor(pet => pet.BehaviourDesc)
            .NotEmpty().WithMessage("A descrição de comportamento não pode ser vazia.")
            .MaximumLength(500).WithMessage("A descrição de comportamento não pode ter mais de 500 caracteres.");

        RuleFor(pet => pet.Id)
            .GreaterThan(0).WithMessage("O id do pet deve ser maior que zero.");
    }
}
