using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MuscleGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MuscleGroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetAll()
    {
        var muscleGroups = await _context.MuscleGroups
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(muscleGroups);
    }
}