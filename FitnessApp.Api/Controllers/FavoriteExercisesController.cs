using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteExercisesController : ControllerBase
{
    private readonly IFavoriteExerciseService _favoriteExerciseService;

    public FavoriteExercisesController(IFavoriteExerciseService favoriteExerciseService)
    {
        _favoriteExerciseService = favoriteExerciseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteExerciseReadDto>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var favorites = await _favoriteExerciseService.GetAllAsync(userId, isAdmin);

        return Ok(favorites);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FavoriteExerciseReadDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var favorite = await _favoriteExerciseService.GetByIdAsync(id, userId, isAdmin);

        if (favorite == null)
        {
            return NotFound("Omiljena vježba nije pronađena.");
        }

        return Ok(favorite);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteExerciseReadDto>> Create([FromBody] FavoriteExerciseCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var created = await _favoriteExerciseService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
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
        var deleted = await _favoriteExerciseService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound("Omiljena vježba nije pronađena.");
        }

        return NoContent();
    }
}