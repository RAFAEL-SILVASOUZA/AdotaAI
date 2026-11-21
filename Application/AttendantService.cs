using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class AttendantService(
    IAttendantRepository attendantRepository,
    IInstitutionRepository institutionRepository,
    IValidator<Attendant> validator)
{
    public async Task<AttendantResponseDto> AddAsync(CreateAttendantDto dto, CancellationToken ct = default)
    {
        var attendant = new Attendant(0, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.InstitutionId);
        await ValidateAsync(attendant, ct);

        if (await institutionRepository.GetByIdAsync(dto.InstitutionId, ct) is null)
            throw new ValidationFailedException(["Instituição informada não encontrada."]);

        if (await attendantRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await attendantRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        await attendantRepository.AddAsync(attendant, ct);

        return AttendantResponseDto.FromEntity(attendant);
    }

    public async Task<AttendantResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var a = await attendantRepository.GetByIdAsync(id, ct);
        return a is null ? null : AttendantResponseDto.FromEntity(a);
    }

    public async Task<IReadOnlyList<AttendantResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await attendantRepository.GetAllAsync(ct);
        return list
            .Select(AttendantResponseDto.FromEntity)
            .ToList();
    }

    public async Task<IReadOnlyList<AttendantResponseDto>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        var list = await attendantRepository.GetByInstitutionIdAsync(institutionId, ct);
        return list
            .Select(AttendantResponseDto.FromEntity)
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAttendantDto dto, CancellationToken ct = default)
    {
        var existing = await attendantRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        var updated = new Attendant(id, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.InstitutionId);
        await ValidateAsync(updated, ct);

        if (await institutionRepository.GetByIdAsync(dto.InstitutionId, ct) is null)
            throw new ValidationFailedException(["Instituição informada não encontrada."]);

        if (existing.Email != dto.Email && await attendantRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await attendantRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        return await attendantRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await attendantRepository.DeleteAsync(id, ct);
    }

    private async Task ValidateAsync(Attendant attendant, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(attendant, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}