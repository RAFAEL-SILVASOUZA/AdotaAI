using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class PetService(IPetRepository repository, IValidator<Pet> validator)
{
    public async Task<int> CreateAsync(CreatePetDto dto, CancellationToken ct = default)
    {
        var pet = new Pet(
            id: 0,
            name: dto.Name,
            species: dto.Species,
            race: dto.Race,
            age: dto.Age,
            isFemale: dto.IsFemale,
            petSize: dto.PetSize,
            photo: dto.Photo,
            vetRecord: dto.VetRecord,
            behaviourDesc: dto.BehaviourDesc,
            petStatus: dto.PetStatus,
            institutionId: dto.InstitutionId);

        await ValidateAsync(pet, ct);

        await repository.AddAsync(pet, ct);
        return pet.Id;
    }

    public async Task<PetResponseDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var pet = await repository.GetByIdAsync(id, ct);
        if (pet is null)
            throw new NotFoundException($"Pet com id {id} não encontrado.");

        return PetResponseDto.FromEntity(pet);
    }

    public async Task<IReadOnlyList<PetResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var pets = await repository.GetAllAsync(ct);
        return pets.Select(PetResponseDto.FromEntity).ToList();
    }

    public async Task<IReadOnlyList<PetResponseDto>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        var pets = await repository.GetByInstitutionIdAsync(institutionId, ct);
        return pets.Select(PetResponseDto.FromEntity).ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdatePetDto dto, CancellationToken ct = default)
    {
        var existingPet = await repository.GetByIdAsync(id, ct);
        if (existingPet is null)
            throw new NotFoundException($"Pet com id {id} não encontrado.");

        var updatedPet = new Pet(
            id: id,
            name: dto.Name,
            species: dto.Species,
            race: dto.Race,
            age: dto.Age,
            isFemale: dto.IsFemale,
            petSize: dto.PetSize,
            photo: dto.Photo,
            vetRecord: dto.VetRecord,
            behaviourDesc: dto.BehaviourDesc,
            petStatus: dto.PetStatus,
            institutionId: existingPet.InstitutionId);

        await ValidateAsync(updatedPet, ct);

        return await repository.UpdateAsync(updatedPet, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await repository.DeleteAsync(id, ct))
            throw new NotFoundException($"Pet com id {id} não encontrado.");

        return true;
    }

    private async Task ValidateAsync(Pet pet, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(pet, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}