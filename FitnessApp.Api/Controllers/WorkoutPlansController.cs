using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutPlansController : BaseApiController
{
    private readonly IWorkoutPlanService _workoutPlanService;

    public WorkoutPlansController(IWorkoutPlanService workoutPlanService)
    {
        _workoutPlanService = workoutPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutPlanReadDto>>> GetAll()
    {
        var plans = await _workoutPlanService.GetAllAsync(CurrentUserId, IsAdmin);
        return Ok(plans);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutPlanReadDto>> GetById(int id)
    {
        var plan = await _workoutPlanService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (plan == null)
        {
            return NotFound("Plan treninga nije pronađen.");
        }

        return Ok(plan);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutPlanReadDto>> Create([FromBody] WorkoutPlanCreateUpdateDto dto)
    {
        var created = await _workoutPlanService.CreateAsync(CurrentUserId, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkoutPlanCreateUpdateDto dto)
    {
        var updated = await _workoutPlanService.UpdateAsync(id, CurrentUserId, IsAdmin, dto);

        if (!updated)
        {
            return NotFound("Plan treninga nije pronađen.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _workoutPlanService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Plan treninga nije pronađen.");
        }

        return NoContent();
    }
}