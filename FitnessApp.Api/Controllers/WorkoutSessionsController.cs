using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkoutSessionsController : ControllerBase
{
    private readonly IWorkoutSessionService _workoutSessionService;

    public WorkoutSessionsController(IWorkoutSessionService workoutSessionService)
    {
        _workoutSessionService = workoutSessionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkoutSessionReadDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var sessions = await _workoutSessionService.GetAllAsync(userId, isAdmin);

        return Ok(sessions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutSessionReadDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var session = await _workoutSessionService.GetByIdAsync(id, userId, isAdmin);

        if (session == null)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<ActionResult<WorkoutSessionReadDto>> Create([FromBody] WorkoutSessionCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");

        try
        {
            var created = await _workoutSessionService.CreateAsync(userId, isAdmin, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] WorkoutSessionUpdateStatusDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var updated = await _workoutSessionService.UpdateStatusAsync(id, userId, isAdmin, dto);

        if (!updated)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return NoContent();
    }

    [HttpPut("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] WorkoutSessionCompleteDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");

        try
        {
            var completed = await _workoutSessionService.CompleteAsync(id, userId, isAdmin, dto);

            if (!completed)
            {
                return NotFound("Sesija treninga nije pronađena.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
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
        var deleted = await _workoutSessionService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound("Sesija treninga nije pronađena.");
        }

        return NoContent();
    }
}