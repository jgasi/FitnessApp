using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExerciseCategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ExerciseCategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetAll()
    {
        var categories = await _context.ExerciseCategories
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(categories);
    }
}