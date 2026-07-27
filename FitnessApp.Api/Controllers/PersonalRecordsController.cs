using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonalRecordsController : ControllerBase
{
    private readonly IPersonalRecordService _personalRecordService;

    public PersonalRecordsController(IPersonalRecordService personalRecordService)
    {
        _personalRecordService = personalRecordService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonalRecordReadDto>>> GetAll(
        [FromQuery] int? exerciseId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var items = await _personalRecordService.GetAllAsync(userId, isAdmin, exerciseId, from, to);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonalRecordReadDto>> GetById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var item = await _personalRecordService.GetByIdAsync(id, userId, isAdmin);

        if (item == null)
        {
            return NotFound("Osobni rekord nije pronađen.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<PersonalRecordReadDto>> Create([FromBody] PersonalRecordCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var created = await _personalRecordService.CreateAsync(userId, dto);
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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonalRecordCreateUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");

        try
        {
            var updated = await _personalRecordService.UpdateAsync(id, userId, isAdmin, dto);

            if (!updated)
            {
                return NotFound("Osobni rekord nije pronađen.");
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
        var deleted = await _personalRecordService.DeleteAsync(id, userId, isAdmin);

        if (!deleted)
        {
            return NotFound("Osobni rekord nije pronađen.");
        }

        return NoContent();
    }
}