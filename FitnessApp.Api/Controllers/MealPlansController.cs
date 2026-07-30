using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class MealPlansController : BaseApiController
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlansController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MealPlanReadDto>>> GetAll()
    {
        var plans = await _mealPlanService.GetAllAsync(CurrentUserId, IsAdmin);
        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MealPlanReadDto>> GetById(int id)
    {
        var plan = await _mealPlanService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (plan == null)
        {
            return NotFound("Plan prehrane nije pronađen.");
        }

        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<MealPlanReadDto>> Create([FromBody] MealPlanCreateUpdateDto dto)
    {
        var created = await _mealPlanService.CreateAsync(CurrentUserId, dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MealPlanCreateUpdateDto dto)
    {
        var updated = await _mealPlanService.UpdateAsync(id, CurrentUserId, IsAdmin, dto);

        if (!updated)
        {
            return NotFound("Plan prehrane nije pronađen.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _mealPlanService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Plan prehrane nije pronađen.");
        }

        return NoContent();
    }
}