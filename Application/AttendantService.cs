using AdotaAI.Application.Dto;
using AdotaAI.Domain;

namespace AdotaAI.Application;

public class AttendantService(IAttendantRepository attendantRepository)
{
    public async Task<AttendantResponseDto> AddAsync(CreateAttendantDto dto, CancellationToken ct = default)
    {
        if (await attendantRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este e-mail.");

        if (await attendantRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe um funcionário cadastrado com este CPF.");

        var attendant = new Attendant(0, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.InstitutionId);
        await attendantRepository.AddAsync(attendant, ct);

        return new AttendantResponseDto(
            attendant.Id,
            attendant.Name,
            attendant.Email,
            attendant.Cpf,
            attendant.InstitutionId
        );
    }

    public async Task<AttendantResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var a = await attendantRepository.GetByIdAsync(id, ct);
        return a is null ? null : new AttendantResponseDto(a.Id, a.Name, a.Email, a.Cpf, a.InstitutionId);
    }

    public async Task<IReadOnlyList<AttendantResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await attendantRepository.GetAllAsync(ct);
        return list
            .Select(a => new AttendantResponseDto(a.Id, a.Name, a.Email, a.Cpf, a.InstitutionId))
            .ToList();
    }

    public async Task<IReadOnlyList<AttendantResponseDto>> GetByInstitutionIdAsync(int institutionId, CancellationToken ct = default)
    {
        var list = await attendantRepository.GetByInstitutionIdAsync(institutionId, ct);
        return list
            .Select(a => new AttendantResponseDto(a.Id, a.Name, a.Email, a.Cpf, a.InstitutionId))
            .ToList();
    }

    public async Task<bool> UpdateAsync(int id, UpdateAttendantDto dto, CancellationToken ct = default)
    {
        var existing = await attendantRepository.GetByIdAsync(id, ct);
        if (existing is null)
            return false;

        if (existing.Email != dto.Email && await attendantRepository.EmailExistsAsync(dto.Email, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este e-mail.");

        if (existing.Cpf != dto.Cpf && await attendantRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new InvalidOperationException("Já existe outro funcionário cadastrado com este CPF.");

        var updated = new Attendant(id, dto.Name, dto.Email, dto.Password, dto.Cpf, dto.InstitutionId);
        return await attendantRepository.UpdateAsync(updated, ct);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        return await attendantRepository.DeleteAsync(id, ct);
    }
}