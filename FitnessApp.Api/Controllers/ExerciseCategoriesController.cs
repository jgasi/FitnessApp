using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseCategoriesController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public ExerciseCategoriesController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetAll()
    {
        var categories = await _lookupService.GetExerciseCategoriesAsync();
        return Ok(categories);
    }
}