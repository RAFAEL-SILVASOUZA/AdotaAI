using AdotaAI.Application.Dto;
using AdotaAI.Domain;

namespace AdotaAI.Application;

public class AdmService(IAdmRepository admRepository)
{
    public async Task<AdmResponseDto> AddAsync(CreateAdmDto dto, CancellationToken ct = default)
    {
        if (await admRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await admRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        var adm = new Adm(0, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await admRepository.AddAsync(adm, ct);

        return new AdmResponseDto(adm.Id, adm.Name, adm.Email, adm.Cpf);
    }

    public async Task<AdmResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var adm = await admRepository.GetByIdAsync(id, ct);
        return adm is null ? null : new AdmResponseDto(adm.Id, adm.Name, adm.Email, adm.Cpf);
    }

    public async Task<IReadOnlyList<AdmResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await admRepository.GetAllAsync(ct);
        return list
            .Select(a => new AdmResponseDto(a.Id, a.Name, a.Email, a.Cpf))
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAdmDto dto, CancellationToken ct = default)
    {
        var existing = await admRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        if (existing.Email != dto.Email && await admRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await admRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        var updated = new Adm(id, dto.Name, dto.Email, dto.Password, dto.Cpf);
        return await admRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await admRepository.DeleteAsync(id, ct);
    }
}