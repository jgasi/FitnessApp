using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FitnessApp.Api.Services.Implementations;

public class LookupService : ILookupService
{
    private const string ExerciseCategoriesCacheKey = "lookup:exerciseCategories";
    private const string MuscleGroupsCacheKey = "lookup:muscleGroups";
    private const string FitnessGoalsCacheKey = "lookup:fitnessGoals";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMemoryCache _cache;
    private readonly ILogger<LookupService> _logger;

    public LookupService(IUnitOfWork unitOfWork, IMemoryCache cache, ILogger<LookupService> logger)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<LookupDto>> GetExerciseCategoriesAsync()
    {
        return await GetOrCreateAsync(
            ExerciseCategoriesCacheKey,
            async () => await _unitOfWork.ExerciseCategories.Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
                .ToListAsync());
    }

    public async Task<IEnumerable<LookupDto>> GetMuscleGroupsAsync()
    {
        return await GetOrCreateAsync(
            MuscleGroupsCacheKey,
            async () => await _unitOfWork.MuscleGroups.Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
                .ToListAsync());
    }

    public async Task<IEnumerable<LookupDto>> GetFitnessGoalsAsync()
    {
        return await GetOrCreateAsync(
            FitnessGoalsCacheKey,
            async () => await _unitOfWork.FitnessGoals.Query()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
                .ToListAsync());
    }

    private async Task<IEnumerable<LookupDto>> GetOrCreateAsync(string cacheKey, Func<Task<List<LookupDto>>> factory)
    {
        if (_cache.TryGetValue(cacheKey, out IEnumerable<LookupDto>? cachedValue) && cachedValue is not null)
        {
            _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);
            return cachedValue;
        }

        _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);

        var data = await factory();

        var options = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromHours(12),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        };

        _cache.Set(cacheKey, data, options);

        return data;
    }
}