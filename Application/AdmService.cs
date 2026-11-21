using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class AdmService(IAdmRepository admRepository, IValidator<Adm> validator)
{
    public async Task<AdmResponseDto> AddAsync(CreateAdmDto dto, CancellationToken ct = default)
    {
        var adm = new Adm(0, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await ValidateAsync(adm, ct);

        if (await admRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await admRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

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

        var updated = new Adm(id, dto.Name, dto.Email, dto.Password, dto.Cpf);
        await ValidateAsync(updated, ct);

        if (existing.Email != dto.Email && await admRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await admRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        return await admRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await admRepository.DeleteAsync(id, ct);
    }

    private async Task ValidateAsync(Adm adm, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(adm, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}