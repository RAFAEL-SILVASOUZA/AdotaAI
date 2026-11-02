using AdotaAI.Application.Dto;
using AdotaAI.Domain;

namespace AdotaAI.Application;

public class VeterinarianService(IVeterinarianRepository veterinarianRepository)
{
    public async Task<VeterinarianResponseDto> AddAsync(CreateVeterinarianDto dto, CancellationToken ct = default)
    {
        if (await veterinarianRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await veterinarianRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        if (await veterinarianRepository.CrmvExistsAsync(dto.Crmv, ct))
            throw new InvalidOperationException("Já existe um veterinário cadastrado com este CRMV.");

        var veterinarian = new Veterinarian(0, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.Crmv, dto.InstitutionId);
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

        if (existing.Email != dto.Email && await veterinarianRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await veterinarianRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        if (existing.Crmv != dto.Crmv && await veterinarianRepository.CrmvExistsAsync(dto.Crmv, ct))
            throw new InvalidOperationException("Já existe outro veterinário cadastrado com este CRMV.");

        var updated = new Veterinarian(id, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.Crmv, dto.InstitutionId);
        return await veterinarianRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await veterinarianRepository.DeleteAsync(id, ct);
    }
}