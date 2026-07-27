using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CalorieEntriesController : ControllerBase
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var items = await _calorieEntryService.GetAllAsync(userId, isAdmin, from, to);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CalorieEntryReadDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var item = await _calorieEntryService.GetByIdAsync(id, userId, isAdmin);

        if (item == null)
        {
            return NotFound("Unos kalorija nije pronađen.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CalorieEntryReadDto>> Create([FromBody] CalorieEntryCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var created = await _calorieEntryService.CreateAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CalorieEntryCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");

        try
        {
            var updated = await _calorieEntryService.UpdateAsync(id, userId, isAdmin, dto);

            if (!updated)
            {
                return NotFound("Unos kalorija nije pronađen.");
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
        var deleted = await _calorieEntryService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound("Unos kalorija nije pronađen.");
        }

        return NoContent();
    }
}