using AdotaAI.Application;
using AdotaAI.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AdotaAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdmController(AdmService admService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AdmResponseDto>> Create([FromBody] CreateAdmDto dto, CancellationToken ct)
    {
        try
        {
            var result = await admService.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AdmResponseDto>>> GetAll(CancellationToken ct)
    {
        var list = await admService.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdmResponseDto>> GetById(int id, CancellationToken ct)
    {
        var result = await admService.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound(new { message = "Administrador não encontrado." });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAdmDto dto, CancellationToken ct)
    {
        try
        {
            var success = await admService.UpdateAsync(id, dto, ct);
            if (!success)
                return NotFound(new { message = "Administrador não encontrado." });

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
        var success = await admService.DeleteAsync(id, ct);
        if (!success)
            return NotFound(new { message = "Administrador não encontrado." });

        return NoContent();
    }
}