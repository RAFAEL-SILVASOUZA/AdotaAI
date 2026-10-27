using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class InstitutionService(IInstitutionRepository repository, IValidator<Institution> validator)
{
    public async Task<int> CreateAsync(CreateInstitutionDto dto, CancellationToken ct = default)
    {
        var institution = new Institution(
            id: 0,
            name: dto.Name,
            email: dto.Email,
            phone: dto.Phone,
            document: dto.Document,
            photo: dto.Photo,
            password: dto.Password,
            description: dto.Description,
            operatingHours: dto.OperatingHours,
            address: dto.Address);

        await ValidateAsync(institution, ct);

        if (await repository.EmailExistsAsync(dto.Email, ct))
            throw new ValidationFailedException(["O email informado já está cadastrado."]);

        await repository.AddAsync(institution, ct);
        return institution.Id;
    }

    public async Task<InstitutionResponseDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var institution = await repository.GetByIdAsync(id, ct);
        if (institution is null)
            throw new NotFoundException($"Instituição com id {id} não encontrada.");

        return InstitutionResponseDto.FromEntity(institution);
    }

    public async Task<IReadOnlyList<InstitutionResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var institutions = await repository.GetAllAsync(ct);
        return institutions.Select(InstitutionResponseDto.FromEntity).ToList();
    }

    public async Task<bool> UpdateAsync(UpdateInstitutionDto dto, CancellationToken ct = default)
    {
        var institution = new Institution(
            id: dto.Id,
            name: dto.Name,
            email: dto.Email,
            phone: dto.Phone,
            document: dto.Document,
            photo: dto.Photo,
            password: dto.Password,
            description: dto.Description,
            operatingHours: dto.OperatingHours,
            address: dto.Address);

        await ValidateAsync(institution, ct);

        if (!await repository.UpdateAsync(institution, ct))
            throw new NotFoundException($"Instituição com id {dto.Id} não encontrada.");

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await repository.DeleteAsync(id, ct))
            throw new NotFoundException($"Instituição com id {id} não encontrada.");

        return true;
    }

    private async Task ValidateAsync(Institution institution, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(institution, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}
