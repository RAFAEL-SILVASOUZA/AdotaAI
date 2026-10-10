using AdotaAI.Application.Dto;
using AdotaAI.Application.Exceptions;
using AdotaAI.Domain;
using FluentValidation;

namespace AdotaAI.Application;

public class UserService(IUserRepository repository, IValidator<User> validator)
{
    public async Task<int> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = new User(
            id: 0,
            name: dto.Name,
            email: dto.Email,
            phone: dto.Phone,
            age: dto.Age,
            photo: dto.Photo,
            password: dto.Password,
            gender: dto.Gender,
            address: dto.Address);

        await ValidateAsync(user, ct);

        if (await repository.EmailExistsAsync(dto.Email, ct))
            throw new ValidationFailedException(["O email informado já está cadastrado."]);

        await repository.AddAsync(user, ct);
        return user.Id;
    }

    public async Task<UserResponseDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await repository.GetByIdAsync(id, ct);
        if (user is null)
            throw new NotFoundException($"Usuário com id {id} não encontrado.");

        return UserResponseDto.FromEntity(user);
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await repository.GetAllAsync(ct);
        return users.Select(UserResponseDto.FromEntity).ToList();
    }

    public async Task<bool> UpdateAsync(UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = new User(
            id: dto.Id,
            name: dto.Name,
            email: dto.Email,
            phone: dto.Phone,
            age: dto.Age,
            photo: dto.Photo,
            password: dto.Password,
            gender: dto.Gender,
            address: dto.Address);

        await ValidateAsync(user, ct);

        if (!await repository.UpdateAsync(user, ct))
            throw new NotFoundException($"Usuário com id {dto.Id} não encontrado.");

        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!await repository.DeleteAsync(id, ct))
            throw new NotFoundException($"Usuário com id {id} não encontrado.");

        return true;
    }

    private async Task ValidateAsync(User user, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(user, ct);
        if (!result.IsValid)
            throw new ValidationFailedException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}
