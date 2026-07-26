using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseController : ControllerBase
{
    private readonly IExerciseService _exerciseService;

    public ExerciseController(IExerciseService exerciseService)
    {
        _exerciseService = exerciseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseReadDto>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] int? exerciseCategoryId = null,
        [FromQuery] int? muscleGroupId = null)
    {
        var exercises = await _exerciseService.GetAllAsync(search, exerciseCategoryId, muscleGroupId);
        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciseReadDto>> GetById(int id)
    {
        var exercise = await _exerciseService.GetByIdAsync(id);

        if (exercise == null)
        {
            return NotFound("Vježba nije pronađena.");
        }

        return Ok(exercise);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ExerciseReadDto>> Create([FromBody] ExerciseCreateUpdateDto dto)
    {
        try
        {
            var created = await _exerciseService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(int id, [FromBody] ExerciseCreateUpdateDto dto)
    {
        try
        {
            var updated = await _exerciseService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound("Vježba nije pronađena.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _exerciseService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound("Vježba nije pronađena.");
        }

        return NoContent();
    }
}