using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/[controller]")]
public class ProfilesController : BaseApiController
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
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

        return NoContent();
    }
}