using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class PersonalRecordsController : BaseApiController
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
        var items = await _personalRecordService.GetAllAsync(
            CurrentUserId,
            IsAdmin,
            exerciseId,
            from,
            to);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PersonalRecordReadDto>> GetById(int id)
    {
        var item = await _personalRecordService.GetByIdAsync(
            id,
            CurrentUserId,
            IsAdmin);

        if (item == null)
        {
            return NotFound("Osobni rekord nije pronađen.");
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<PersonalRecordReadDto>> Create([FromBody] PersonalRecordCreateUpdateDto dto)
    {
        var created = await _personalRecordService.CreateAsync(CurrentUserId, dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonalRecordCreateUpdateDto dto)
    {
        var updated = await _personalRecordService.UpdateAsync(
            id,
            CurrentUserId,
            IsAdmin,
            dto);

        if (!updated)
        {
            return NotFound("Osobni rekord nije pronađen.");
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _personalRecordService.DeleteAsync(
            id,
            CurrentUserId,
            IsAdmin);

        if (!deleted)
        {
            return NotFound("Osobni rekord nije pronađen.");
        }

        return NoContent();
    }
}