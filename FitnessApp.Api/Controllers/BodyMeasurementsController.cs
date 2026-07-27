using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BodyMeasurementsController : ControllerBase
{
    private readonly IBodyMeasurementService _bodyMeasurementService;

    public BodyMeasurementsController(IBodyMeasurementService bodyMeasurementService)
    {
        _bodyMeasurementService = bodyMeasurementService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BodyMeasurementReadDto>>> GetAll(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var items = await _bodyMeasurementService.GetAllAsync(userId, isAdmin, from, to);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BodyMeasurementReadDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var item = await _bodyMeasurementService.GetByIdAsync(id, userId, isAdmin);

        if (item == null)
        {
            return NotFound("Mjerenje nije pronađeno.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BodyMeasurementReadDto>> Create([FromBody] BodyMeasurementCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var created = await _bodyMeasurementService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BodyMeasurementCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");

        try
        {
            var updated = await _bodyMeasurementService.UpdateAsync(id, userId, isAdmin, dto);

            if (!updated)
            {
                return NotFound("Mjerenje nije pronađeno.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var deleted = await _bodyMeasurementService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound("Mjerenje nije pronađeno.");
        }

        return NoContent();
    }
}