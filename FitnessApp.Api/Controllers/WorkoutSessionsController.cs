using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutSessionsController : BaseApiController
{
    private readonly IWorkoutSessionService _workoutSessionService;

    public WorkoutSessionsController(IWorkoutSessionService workoutSessionService)
    {
        _workoutSessionService = workoutSessionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutSessionReadDto>>> GetAll()
    {
        var sessions = await _workoutSessionService.GetAllAsync(CurrentUserId, IsAdmin);
        return Ok(sessions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutSessionReadDto>> GetById(int id)
    {
        var session = await _workoutSessionService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (session == null)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSessionReadDto>> Create([FromBody] WorkoutSessionCreateDto dto)
    {
        var created = await _workoutSessionService.CreateAsync(CurrentUserId, IsAdmin, dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] WorkoutSessionUpdateStatusDto dto)
    {
        var updated = await _workoutSessionService.UpdateStatusAsync(id, CurrentUserId, IsAdmin, dto);

        if (!updated)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return NoContent();
    }

    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] WorkoutSessionCompleteDto dto)
    {
        var completed = await _workoutSessionService.CompleteAsync(id, CurrentUserId, IsAdmin, dto);

        if (!completed)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _workoutSessionService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return NoContent();
    }
}