using AdotaAI.Application;
using AdotaAI.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AdotaAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto dto, CancellationToken ct)
    {
        var id = await service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        return Ok(await service.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        return Ok(await service.GetByIdAsync(id, ct));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateUserDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
