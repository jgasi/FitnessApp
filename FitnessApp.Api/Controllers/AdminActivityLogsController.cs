using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/admin/activity-logs")]
[Authorize(Roles = "Administrator")]
public class AdminActivityLogsController : BaseApiController
{
    private readonly IActivityLogService _activityLogService;

    public AdminActivityLogsController(IActivityLogService activityLogService)
    {
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ActivityLogReadDto>>> GetAll(
        [FromQuery] string? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] string? entityName = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int take = 100)
    {
        var logs = await _activityLogService.GetAllAsync(userId, action, entityName, from, to, take);
        return Ok(logs);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ActivityLogReadDto>> GetById(int id)
    {
        var log = await _activityLogService.GetByIdAsync(id);

        if (log == null)
        {
            return NotFound("Zapis aktivnosti nije pronađen.");
        }

        return Ok(log);
    }
}