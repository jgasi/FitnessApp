using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class FavoriteExercisesController : BaseApiController
{
    private readonly IFavoriteExerciseService _favoriteExerciseService;

    public FavoriteExercisesController(IFavoriteExerciseService favoriteExerciseService)
    {
        _favoriteExerciseService = favoriteExerciseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteExerciseReadDto>>> GetAll()
    {
        var favorites = await _favoriteExerciseService.GetAllAsync(CurrentUserId, IsAdmin);
        return Ok(favorites);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FavoriteExerciseReadDto>> GetById(int id)
    {
        var favorite = await _favoriteExerciseService.GetByIdAsync(id, CurrentUserId, IsAdmin);

        if (favorite == null)
        {
            return NotFound("Omiljena vježba nije pronađena.");
        }

        return Ok(favorite);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteExerciseReadDto>> Create([FromBody] FavoriteExerciseCreateDto dto)
    {
        var created = await _favoriteExerciseService.CreateAsync(CurrentUserId, dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _favoriteExerciseService.DeleteAsync(id, CurrentUserId, IsAdmin);

        if (!deleted)
        {
            return NotFound("Omiljena vježba nije pronađena.");
        }

        return NoContent();
    }
}