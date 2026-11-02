using AdotaAI.Application;
using AdotaAI.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AdotaAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VeterinarianController(VeterinarianService veterinarianService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<VeterinarianResponseDto>> Create([FromBody] CreateVeterinarianDto dto, CancellationToken ct)
    {
        try
        {
            var result = await veterinarianService.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VeterinarianResponseDto>>> GetAll(CancellationToken ct)
    {
        var list = await veterinarianService.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VeterinarianResponseDto>> GetById(int id, CancellationToken ct)
    {
        var result = await veterinarianService.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound(new { message = "Veterinário não encontrado." });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVeterinarianDto dto, CancellationToken ct)
    {
        try
        {
            var success = await veterinarianService.UpdateAsync(id, dto, ct);
            if (!success)
                return NotFound(new { message = "Veterinário não encontrado." });

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await veterinarianService.DeleteAsync(id, ct);
        if (!success)
            return NotFound(new { message = "Veterinário não encontrado." });

        return NoContent();
    }
}