using AdotaAI.Application;
using AdotaAI.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AdotaAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendantController(AttendantService attendantService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AttendantResponseDto>> Create([FromBody] CreateAttendantDto dto, CancellationToken ct)
    {
        try
        {
            var result = await attendantService.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AttendantResponseDto>>> GetAll(CancellationToken ct)
    {
        var list = await attendantService.GetAllAsync(ct);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AttendantResponseDto>> GetById(int id, CancellationToken ct)
    {
        var result = await attendantService.GetByIdAsync(id, ct);
        if (result is null)
            return NotFound(new { message = "Atendente não encontrado." });

        return Ok(result);
    }

    [HttpGet("institution/{institutionId:int}")]
    public async Task<ActionResult<IReadOnlyList<AttendantResponseDto>>> GetByInstitutionId(int institutionId, CancellationToken ct)
    {
        var list = await attendantService.GetByInstitutionIdAsync(institutionId, ct);
        return Ok(list);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendantDto dto, CancellationToken ct)
    {
        try
        {
            var success = await attendantService.UpdateAsync(id, dto, ct);
            if (!success)
                return NotFound(new { message = "Atendente não encontrado." });

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
        var success = await attendantService.DeleteAsync(id, ct);
        if (!success)
            return NotFound(new { message = "Atendente não encontrado." });

        return NoContent();
    }
}