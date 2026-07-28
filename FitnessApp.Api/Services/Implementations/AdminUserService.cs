using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AdminUserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IEnumerable<AdminUserReadDto>> GetAllAsync()
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .OrderBy(x => x.UserName)
            .ToListAsync();

        var profiles = await _context.UserProfiles
            .AsNoTracking()
            .Include(x => x.FitnessGoal)
            .ToListAsync();

        var profileByUserId = profiles.ToDictionary(x => x.UserId, x => x);

        var result = new List<AdminUserReadDto>();

        foreach (var user in users)
        {
            profileByUserId.TryGetValue(user.Id, out var profile);
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(MapToDto(user, roles.ToList(), profile));
        }

        return result;
    }

    public async Task<AdminUserReadDto?> GetByIdAsync(string userId)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            return null;
        }

        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        var profile = await _context.UserProfiles
            .AsNoTracking()
            .Include(x => x.FitnessGoal)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        return MapToDto(user, roles, profile);
    }

    public async Task<bool> UpdateRoleAsync(string targetUserId, string currentUserId, AdminUserRoleUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Role))
        {
            throw new ArgumentException("Rola je obavezna.");
        }

        var roleName = dto.Role.Trim();

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            throw new ArgumentException("Rola ne postoji.");
        }

        var user = await _userManager.FindByIdAsync(targetUserId);

        if (user == null)
        {
            return false;
        }

        if (user.Id == currentUserId && roleName != "Administrator")
        {
            throw new InvalidOperationException("Ne možeš promijeniti vlastitu administratorsku rolu.");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Contains(roleName) && currentRoles.Count == 1)
        {
            return true;
        }

        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return false;
        }

        var addResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addResult.Succeeded)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateStatusAsync(string targetUserId, string currentUserId, AdminUserStatusUpdateDto dto)
    {
        var user = await _userManager.FindByIdAsync(targetUserId);

        if (user == null)
        {
            return false;
        }

        if (user.Id == currentUserId && dto.IsActive == false)
        {
            throw new InvalidOperationException("Ne možeš deaktivirati vlastiti račun.");
        }

        user.IsActive = dto.IsActive;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    private static AdminUserReadDto MapToDto(
        ApplicationUser user,
        List<string> roles,
        UserProfile? profile)
    {
        return new AdminUserReadDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = roles,
            FitnessGoalId = profile?.FitnessGoalId,
            FitnessGoalName = profile?.FitnessGoal?.Name,
            DateOfBirth = profile?.DateOfBirth,
            Gender = profile?.Gender,
            HeightCm = profile?.HeightCm,
            CurrentWeightKg = profile?.CurrentWeightKg
        };
    }
}