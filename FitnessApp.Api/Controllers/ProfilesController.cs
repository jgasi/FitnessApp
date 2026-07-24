using System.Security.Claims;
using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfilesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileReadDto>> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var profile = await _context.UserProfiles
            .AsNoTracking()
            .Include(x => x.FitnessGoal)
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .Select(x => new UserProfileReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Email = x.User.Email ?? string.Empty,
                FitnessGoalId = x.FitnessGoalId,
                FitnessGoalName = x.FitnessGoal != null ? x.FitnessGoal.Name : null,
                DateOfBirth = x.DateOfBirth,
                Gender = x.Gender,
                HeightCm = x.HeightCm,
                CurrentWeightKg = x.CurrentWeightKg
            })
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            return NotFound("Profil nije pronađen.");
        }

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return NotFound("Profil nije pronađen.");
        }

        if (dto.FitnessGoalId.HasValue)
        {
            var goalExists = await _context.FitnessGoals.AnyAsync(x => x.Id == dto.FitnessGoalId.Value);
            if (!goalExists)
            {
                return BadRequest("Fitness cilj ne postoji.");
            }
        }

        profile.FitnessGoalId = dto.FitnessGoalId;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.Gender = dto.Gender;
        profile.HeightCm = dto.HeightCm;
        profile.CurrentWeightKg = dto.CurrentWeightKg;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}