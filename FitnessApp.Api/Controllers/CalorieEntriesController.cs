using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class CalorieEntriesController : BaseApiController
{
    private readonly ICalorieEntryService _calorieEntryService;

    public CalorieEntriesController(ICalorieEntryService calorieEntryService)
    {
        _calorieEntryService = calorieEntryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CalorieEntryReadDto>>> GetAll(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var items = await _calorieEntryService.GetAllAsync(CurrentUserId, IsAdmin, from, to);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CalorieEntryReadDto>> GetById(int id)
    {
        var item = await _calorieEntryService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (item == null)
        {
            return NotFound("Unos kalorija nije pronađen.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CalorieEntryReadDto>> Create([FromBody] CalorieEntryCreateUpdateDto dto)
    {
        var created = await _calorieEntryService.CreateAsync(CurrentUserId, dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CalorieEntryCreateUpdateDto dto)
    {
        var updated = await _calorieEntryService.UpdateAsync(id, CurrentUserId, IsAdmin, dto);

        if (!updated)
        {
            return NotFound("Unos kalorija nije pronađen.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _calorieEntryService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Unos kalorija nije pronađen.");
        }

        return NoContent();
    }
}