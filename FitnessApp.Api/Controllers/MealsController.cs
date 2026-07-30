using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class MealsController : BaseApiController
{
    private readonly IMealService _mealService;

    public MealsController(IMealService mealService)
    {
        _mealService = mealService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MealReadDto>>> GetAll()
    {
        var items = await _mealService.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MealReadDto>> GetById(int id)
    {
        var item = await _mealService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound("Obrok nije pronađen.");
        }

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<MealReadDto>> Create([FromBody] MealCreateUpdateDto dto)
    {
        var created = await _mealService.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(int id, [FromBody] MealCreateUpdateDto dto)
    {
        var updated = await _mealService.UpdateAsync(id, dto);

        if (!updated)
        {
            return NotFound("Obrok nije pronađen.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mealService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Obrok nije pronađen.");
        }

        return NoContent();
    }
}