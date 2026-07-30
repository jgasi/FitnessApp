using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class FitnessGoalsController : BaseApiController
{
    private readonly ILookupService _lookupService;

    public FitnessGoalsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetAll()
    {
        var goals = await _lookupService.GetFitnessGoalsAsync();
        return Ok(goals);
    }
}