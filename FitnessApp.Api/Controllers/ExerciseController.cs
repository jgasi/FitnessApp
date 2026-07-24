using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExerciseController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExerciseReadDto>>> GetAll(
    [FromQuery] string? search = null,
    [FromQuery] int? exerciseCategoryId = null,
    [FromQuery] int? muscleGroupId = null)
    {
        var query = _context.Exercises
            .AsNoTracking()
            .Include(e => e.ExerciseCategory)
            .Include(e => e.MuscleGroup)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search));
        }

        if (exerciseCategoryId.HasValue)
        {
            query = query.Where(e => e.ExerciseCategoryId == exerciseCategoryId.Value);
        }

        if (muscleGroupId.HasValue)
        {
            query = query.Where(e => e.MuscleGroupId == muscleGroupId.Value);
        }

        var exercises = await query
            .Select(e => new ExerciseReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                YoutubeUrl = e.YoutubeUrl,
                ExerciseCategoryId = e.ExerciseCategoryId,
                ExerciseCategoryName = e.ExerciseCategory.Name,
                MuscleGroupId = e.MuscleGroupId,
                MuscleGroupName = e.MuscleGroup.Name
            })
            .ToListAsync();

        return Ok(exercises);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciseReadDto>> GetById(int id)
    {
        var exercise = await _context.Exercises
            .AsNoTracking()
            .Include(e => e.ExerciseCategory)
            .Include(e => e.MuscleGroup)
            .Where(e => e.Id == id)
            .Select(e => new ExerciseReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                YoutubeUrl = e.YoutubeUrl,
                ExerciseCategoryId = e.ExerciseCategoryId,
                ExerciseCategoryName = e.ExerciseCategory.Name,
                MuscleGroupId = e.MuscleGroupId,
                MuscleGroupName = e.MuscleGroup.Name
            })
            .FirstOrDefaultAsync();

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
        var categoryExists = await _context.ExerciseCategories.AnyAsync(x => x.Id == dto.ExerciseCategoryId);
        if (!categoryExists)
        {
            return BadRequest("Kategorija vježbe ne postoji.");
        }

        var muscleGroupExists = await _context.MuscleGroups.AnyAsync(x => x.Id == dto.MuscleGroupId);
        if (!muscleGroupExists)
        {
            return BadRequest("Mišićna skupina ne postoji.");
        }

        var exercise = new Exercise
        {
            Name = dto.Name,
            Description = dto.Description,
            YoutubeUrl = dto.YoutubeUrl,
            ExerciseCategoryId = dto.ExerciseCategoryId,
            MuscleGroupId = dto.MuscleGroupId
        };

        _context.Exercises.Add(exercise);
        await _context.SaveChangesAsync();

        var result = await _context.Exercises
            .AsNoTracking()
            .Include(e => e.ExerciseCategory)
            .Include(e => e.MuscleGroup)
            .Where(e => e.Id == exercise.Id)
            .Select(e => new ExerciseReadDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                YoutubeUrl = e.YoutubeUrl,
                ExerciseCategoryId = e.ExerciseCategoryId,
                ExerciseCategoryName = e.ExerciseCategory.Name,
                MuscleGroupId = e.MuscleGroupId,
                MuscleGroupName = e.MuscleGroup.Name
            })
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(int id, [FromBody] ExerciseCreateUpdateDto dto)
    {
        var exercise = await _context.Exercises.FindAsync(id);

        if (exercise == null)
        {
            return NotFound("Vježba nije pronađena.");
        }

        var categoryExists = await _context.ExerciseCategories.AnyAsync(x => x.Id == dto.ExerciseCategoryId);
        if (!categoryExists)
        {
            return BadRequest("Kategorija vježbe ne postoji.");
        }

        var muscleGroupExists = await _context.MuscleGroups.AnyAsync(x => x.Id == dto.MuscleGroupId);
        if (!muscleGroupExists)
        {
            return BadRequest("Mišićna skupina ne postoji.");
        }

        exercise.Name = dto.Name;
        exercise.Description = dto.Description;
        exercise.YoutubeUrl = dto.YoutubeUrl;
        exercise.ExerciseCategoryId = dto.ExerciseCategoryId;
        exercise.MuscleGroupId = dto.MuscleGroupId;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var exercise = await _context.Exercises.FindAsync(id);

        if (exercise == null)
        {
            return NotFound("Vježba nije pronađena.");
        }

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}