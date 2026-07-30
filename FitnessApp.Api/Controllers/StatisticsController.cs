using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class StatisticsController : BaseApiController
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    [HttpGet("me/overview")]
    public async Task<ActionResult<StatisticsOverviewDto>> GetMyOverview()
    {
        var overview = await _statisticsService.GetMyOverviewAsync(CurrentUserId, IsAdmin);
        return Ok(overview);
    }

    [HttpGet("me/weight-progress")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetWeightProgress(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var data = await _statisticsService.GetWeightProgressAsync(CurrentUserId, IsAdmin, from, to);
        return Ok(data);
    }

    [HttpGet("me/calories-progress")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetCaloriesProgress(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var data = await _statisticsService.GetCaloriesProgressAsync(CurrentUserId, IsAdmin, from, to);
        return Ok(data);
    }

    [HttpGet("me/workouts-by-month")]
    public async Task<ActionResult<IEnumerable<StatisticsPointDto>>> GetWorkoutCountsByMonth(
        [FromQuery] int? year = null)
    {
        var data = await _statisticsService.GetWorkoutCountsByMonthAsync(CurrentUserId, IsAdmin, year);
        return Ok(data);
    }

    [HttpGet("me/top-exercises")]
    public async Task<ActionResult<IEnumerable<StatisticsTopExerciseDto>>> GetTopExercises(
        [FromQuery] int take = 5)
    {
        var data = await _statisticsService.GetTopExercisesAsync(CurrentUserId, IsAdmin, take);
        return Ok(data);
    }

    [HttpGet("me/recent-records")]
    public async Task<ActionResult<IEnumerable<StatisticsRecentPersonalRecordDto>>> GetRecentRecords(
        [FromQuery] int take = 10)
    {
        var data = await _statisticsService.GetRecentPersonalRecordsAsync(CurrentUserId, IsAdmin, take);
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