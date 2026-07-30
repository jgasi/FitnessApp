using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class BodyMeasurementsController : BaseApiController
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
        var items = await _bodyMeasurementService.GetAllAsync(CurrentUserId, IsAdmin, from, to);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BodyMeasurementReadDto>> GetById(int id)
    {
        var item = await _bodyMeasurementService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (item == null)
        {
            return NotFound("Mjerenje nije pronađeno.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<BodyMeasurementReadDto>> Create([FromBody] BodyMeasurementCreateUpdateDto dto)
    {
        var created = await _bodyMeasurementService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BodyMeasurementCreateUpdateDto dto)
    {
        var updated = await _bodyMeasurementService.UpdateAsync(id, CurrentUserId, IsAdmin, dto);

        if (!updated)
        {
            return NotFound("Mjerenje nije pronađeno.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _bodyMeasurementService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Mjerenje nije pronađeno.");
        }

        return NoContent();
    }
}