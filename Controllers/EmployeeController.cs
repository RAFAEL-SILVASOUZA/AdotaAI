using AdotaAI.Application;
using AdotaAI.Application.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AdotaAI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController(EmployeeService employeeService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> Create([FromBody] CreateEmployeeDto dto, CancellationToken ct)
    {
        try
        {
            var result = await employeeService.AddAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmployeeResponseDto>>> GetAll(CancellationToken ct)
    {
        var employees = await employeeService.GetAllAsync(ct);
        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(int id, CancellationToken ct)
    {
        var employee = await employeeService.GetByIdAsync(id, ct);
        if (employee is null)
            return NotFound(new { message = "Funcionário não encontrado." });

        return Ok(employee);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDto dto, CancellationToken ct)
    {
        try
        {
            var success = await employeeService.UpdateAsync(id, dto, ct);
            if (!success)
                return NotFound(new { message = "Funcionário não encontrado." });

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
        var success = await employeeService.DeleteAsync(id, ct);
        if (!success)
            return NotFound(new { message = "Funcionário não encontrado." });

        return NoContent();
    }
}