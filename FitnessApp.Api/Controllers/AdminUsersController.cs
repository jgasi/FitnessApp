using System.Security.Claims;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Administrator")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminUserService _adminUserService;

    public AdminUsersController(IAdminUserService adminUserService)
    {
        _adminUserService = adminUserService;
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
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return Unauthorized();
        }

        try
        {
            var updated = await _adminUserService.UpdateRoleAsync(userId, currentUserId, dto);

            if (!updated)
            {
                return NotFound("Korisnik nije pronađen.");
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{userId}/status")]
    public async Task<IActionResult> UpdateStatus(string userId, [FromBody] AdminUserStatusUpdateDto dto)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return Unauthorized();
        }

        try
        {
            var updated = await _adminUserService.UpdateStatusAsync(userId, currentUserId, dto);

            if (!updated)
            {
                return NotFound("Korisnik nije pronađen.");
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}