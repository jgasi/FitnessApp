using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("me/overview")]
    public async Task<ActionResult<StatisticsOverviewDto>> GetMyOverview()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var overview = await _statisticsService.GetMyOverviewAsync(userId, isAdmin);

        return Ok(overview);
    }

    [HttpGet("me/weight-progress")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetWeightProgress(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var data = await _statisticsService.GetWeightProgressAsync(userId, isAdmin, from, to);

        return Ok(data);
    }

    [HttpGet("me/calories-progress")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetCaloriesProgress(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var data = await _statisticsService.GetCaloriesProgressAsync(userId, isAdmin, from, to);

        return Ok(data);
    }

    [HttpGet("me/workouts-by-month")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetWorkoutCountsByMonth([FromQuery] int? year = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var data = await _statisticsService.GetWorkoutCountsByMonthAsync(userId, isAdmin, year);

        return Ok(data);
    }

    [HttpGet("me/top-exercises")]
    public async Task<ActionResult<IEnumerable<StatisticsTopExerciseDto>>> GetTopExercises([FromQuery] int take = 5)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var data = await _statisticsService.GetTopExercisesAsync(userId, isAdmin, take);

        return Ok(data);
    }

    [HttpGet("me/recent-records")]
    public async Task<ActionResult<IEnumerable<StatisticsRecentPersonalRecordDto>>> GetRecentRecords([FromQuery] int take = 10)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Administrator");
        var data = await _statisticsService.GetRecentPersonalRecordsAsync(userId, isAdmin, take);

        return Ok(data);
    }

    [HttpGet("admin/overview")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<AdminStatisticsOverviewDto>> GetAdminOverview()
    {
        var overview = await _statisticsService.GetAdminOverviewAsync();
        return Ok(overview);
    }
}