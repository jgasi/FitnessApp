using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class ActivityLogService : IActivityLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityLogService> _logger;

    public ActivityLogService(IUnitOfWork unitOfWork, ILogger<ActivityLogService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task LogAsync(
        string userId,
        string action,
        string entityName,
        string? entityId = null,
        string? details = null,
        string? ipAddress = null)
    {
        try
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress
            };

            await _unitOfWork.ActivityLogs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ne mogu spremiti activity log za {Action} na {EntityName}", action, entityName);
        }
    }

    public async Task<IEnumerable<ActivityLogReadDto>> GetAllAsync(
        string? userId = null,
        string? action = null,
        string? entityName = null,
        DateTime? from = null,
        DateTime? to = null,
        int take = 100)
    {
        if (take <= 0)
        {
            take = 100;
        }

        if (take > 500)
        {
            take = 500;
        }

        var query = _unitOfWork.ActivityLogs.Query()
            .AsNoTracking()
            .Include(x => x.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userId))
        {
            query = query.Where(x => x.UserId == userId);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(x => x.Action.Contains(action));
        }

        if (!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(x => x.EntityName.Contains(entityName));
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= to.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new ActivityLogReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName ?? string.Empty,
                Email = x.User.Email ?? string.Empty,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                Details = x.Details,
                IpAddress = x.IpAddress,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<ActivityLogReadDto?> GetByIdAsync(int id)
    {
        return await _unitOfWork.ActivityLogs.Query()
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.Id == id)
            .Select(x => new ActivityLogReadDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.User.UserName ?? string.Empty,
                Email = x.User.Email ?? string.Empty,
                Action = x.Action,
                EntityName = x.EntityName,
                EntityId = x.EntityId,
                Details = x.Details,
                IpAddress = x.IpAddress,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();
    }
}