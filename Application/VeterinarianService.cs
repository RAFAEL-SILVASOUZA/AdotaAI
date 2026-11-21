using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class VeterinarianService(
    IVeterinarianRepository veterinarianRepository,
    IInstitutionRepository institutionRepository,
    IValidator<Veterinarian> validator)
{
    public async Task<VeterinarianResponseDto> AddAsync(CreateVeterinarianDto dto, CancellationToken ct = default)
    {
        var veterinarian = new Veterinarian(0, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.Crmv, dto.InstitutionId);
        await ValidateAsync(veterinarian, ct);

        if (await institutionRepository.GetByIdAsync(dto.InstitutionId, ct) is null)
            throw new ValidationFailedException(["Instituição informada não encontrada."]);

        if (await veterinarianRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await veterinarianRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        if (await veterinarianRepository.CrmvExistsAsync(dto.Crmv, ct))
            throw new InvalidOperationException("Já existe um veterinário cadastrado com este CRMV.");

        await veterinarianRepository.AddAsync(veterinarian, ct);

        return new VeterinarianResponseDto(veterinarian.Id, veterinarian.Name, veterinarian.Email, veterinarian.Cpf, veterinarian.Crmv, veterinarian.InstitutionId);
    }

    public async Task<VeterinarianResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var v = await veterinarianRepository.GetByIdAsync(id, ct);
        return v is null ? null : new VeterinarianResponseDto(v.Id, v.Name, v.Email, v.Cpf, v.Crmv, v.InstitutionId);
    }

    public async Task<IReadOnlyList<VeterinarianResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await veterinarianRepository.GetAllAsync(ct);
        return list
            .Select(v => new VeterinarianResponseDto(v.Id, v.Name, v.Email, v.Cpf, v.Crmv, v.InstitutionId))
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateVeterinarianDto dto, CancellationToken ct = default)
    {
        var existing = await veterinarianRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        var updated = new Veterinarian(id, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.Crmv, dto.InstitutionId);
        await ValidateAsync(updated, ct);

        if (await institutionRepository.GetByIdAsync(dto.InstitutionId, ct) is null)
            throw new ValidationFailedException(["Instituição informada não encontrada."]);

        if (existing.Email != dto.Email && await veterinarianRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await veterinarianRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        if (existing.Crmv != dto.Crmv && await veterinarianRepository.CrmvExistsAsync(dto.Crmv, ct))
            throw new InvalidOperationException("Já existe outro veterinário cadastrado com este CRMV.");

        return await veterinarianRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await veterinarianRepository.DeleteAsync(id, ct);
    }

    private async Task ValidateAsync(Veterinarian veterinarian, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(veterinarian, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}