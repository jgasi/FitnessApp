using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IAdminUserService
{
    Task<IEnumerable<AdminUserReadDto>> GetAllAsync();
    Task<AdminUserReadDto?> GetByIdAsync(string userId);
    Task<bool> UpdateRoleAsync(string targetUserId, string currentUserId, AdminUserRoleUpdateDto dto);
    Task<bool> UpdateStatusAsync(string targetUserId, string currentUserId, AdminUserStatusUpdateDto dto);
}