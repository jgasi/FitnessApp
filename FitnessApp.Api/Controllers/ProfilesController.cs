using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class ProfilesController : BaseApiController
{
    private readonly IProfileService _profileService;
    private readonly IActivityLogService _activityLogService;

    public ProfilesController(
        IProfileService profileService,
        IActivityLogService activityLogService)
    {
        _profileService = profileService;
        _activityLogService = activityLogService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileReadDto>> GetMyProfile()
    {
        var profile = await _profileService.GetMyProfileAsync(CurrentUserId);

        if (profile == null)
        {
            return NotFound("Profil nije pronađen.");
        }

        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateDto dto)
    {
        var updated = await _profileService.UpdateMyProfileAsync(CurrentUserId, dto);

        if (!updated)
        {
            return NotFound("Profil nije pronađen.");
        }

        await _activityLogService.LogAsync(
            CurrentUserId,
            "ProfileUpdated",
            "UserProfile",
            CurrentUserId,
            "Korisnik je ažurirao svoj profil.");

        return NoContent();
    }
}