using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MuscleGroupsController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public MuscleGroupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetAll()
    {
        var muscleGroups = await _lookupService.GetMuscleGroupsAsync();
        return Ok(muscleGroups);
    }
}