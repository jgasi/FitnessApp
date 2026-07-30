using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IActivityLogService
{
    Task LogAsync(
        string userId,
        string action,
        string entityName,
        string? entityId = null,
        string? details = null,
        string? ipAddress = null);

    Task<IEnumerable<ActivityLogReadDto>> GetAllAsync(
        string? userId = null,
        string? action = null,
        string? entityName = null,
        DateTime? from = null,
        DateTime? to = null,
        int take = 100);

    Task<ActivityLogReadDto?> GetByIdAsync(int id);
}