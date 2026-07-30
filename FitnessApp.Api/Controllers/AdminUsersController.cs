using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[Route("api/admin/users")]
[Authorize(Roles = "Administrator")]
public class AdminUsersController : BaseApiController
{
    private readonly IAdminUserService _adminUserService;
    private readonly IActivityLogService _activityLogService;

    public AdminUsersController(
        IAdminUserService adminUserService,
        IActivityLogService activityLogService)
    {
        _adminUserService = adminUserService;
        _activityLogService = activityLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminUserReadDto>>> GetAll()
    {
        var users = await _adminUserService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<AdminUserReadDto>> GetById(string userId)
    {
        var user = await _adminUserService.GetByIdAsync(userId);

        if (user == null)
        {
            return NotFound("Korisnik nije pronađen.");
        }

        return Ok(user);
    }

    [HttpPut("{userId}/role")]
    public async Task<IActionResult> UpdateRole(string userId, [FromBody] AdminUserRoleUpdateDto dto)
    {
        var updated = await _adminUserService.UpdateRoleAsync(userId, CurrentUserId, dto);

        if (!updated)
        {
            return NotFound("Korisnik nije pronađen.");
        }

        await _activityLogService.LogAsync(
            CurrentUserId,
            "RoleChanged",
            "ApplicationUser",
            userId,
            $"Promijenjena uloga korisnika u '{dto.Role}'.");

        return NoContent();
    }

    [HttpPut("{userId}/status")]
    public async Task<IActionResult> UpdateStatus(string userId, [FromBody] AdminUserStatusUpdateDto dto)
    {
        var updated = await _adminUserService.UpdateStatusAsync(userId, CurrentUserId, dto);

        if (!updated)
        {
            return NotFound("Korisnik nije pronađen.");
        }

        await _activityLogService.LogAsync(
            CurrentUserId,
            "StatusChanged",
            "ApplicationUser",
            userId,
            dto.IsActive
                ? "Korisnički račun je aktiviran."
                : "Korisnički račun je deaktiviran.");

        return NoContent();
    }
}