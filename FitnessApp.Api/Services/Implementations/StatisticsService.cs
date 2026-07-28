using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<StatisticsOverviewDto> GetMyOverviewAsync(string userId, bool isAdmin)
    {
        var workoutQuery = BuildWorkoutSessionQuery(userId, isAdmin);
        var measurementQuery = BuildBodyMeasurementQuery(userId, isAdmin);
        var calorieQuery = BuildCalorieEntryQuery(userId, isAdmin);
        var personalRecordQuery = BuildPersonalRecordQuery(userId, isAdmin);

        var totalWorkoutSessions = await workoutQuery.CountAsync();
        var completedWorkoutSessions = await workoutQuery.CountAsync(x => x.Status == WorkoutSessionStatus.Completed);
        var totalBodyMeasurements = await measurementQuery.CountAsync();
        var totalCalorieEntries = await calorieQuery.CountAsync();
        var totalPersonalRecords = await personalRecordQuery.CountAsync();

        decimal? startWeight = await measurementQuery
            .OrderBy(x => x.MeasurementDate)
            .Select(x => (decimal?)x.WeightKg)
            .FirstOrDefaultAsync();

        decimal? currentWeight = await measurementQuery
            .OrderByDescending(x => x.MeasurementDate)
            .Select(x => (decimal?)x.WeightKg)
            .FirstOrDefaultAsync();

        decimal? averageCalories = await calorieQuery
            .Select(x => (decimal?)x.Calories)
            .AverageAsync();

        decimal? weightChange = null;
        if (startWeight.HasValue && currentWeight.HasValue)
        {
            weightChange = currentWeight.Value - startWeight.Value;
        }

        return new StatisticsOverviewDto
        {
            TotalWorkoutSessions = totalWorkoutSessions,
            CompletedWorkoutSessions = completedWorkoutSessions,
            TotalBodyMeasurements = totalBodyMeasurements,
            TotalCalorieEntries = totalCalorieEntries,
            TotalPersonalRecords = totalPersonalRecords,
            StartWeightKg = startWeight,
            CurrentWeightKg = currentWeight,
            WeightChangeKg = weightChange,
            AverageCalories = averageCalories
        };
    }

    public async Task<IEnumerable<StatisticsPointDto>> GetWeightProgressAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null)
    {
        var query = BuildBodyMeasurementQuery(userId, isAdmin);

        if (from.HasValue)
        {
            query = query.Where(x => x.MeasurementDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.MeasurementDate <= to.Value);
        }

        var data = await query
            .OrderBy(x => x.MeasurementDate)
            .Select(x => new
            {
                x.MeasurementDate,
                x.WeightKg
            })
            .ToListAsync();

        return data.Select(x => new StatisticsPointDto
        {
            Label = x.MeasurementDate.ToString("yyyy-MM-dd"),
            Value = x.WeightKg
        });
    }

    public async Task<IEnumerable<StatisticsPointDto>> GetCaloriesProgressAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null)
    {
        var query = BuildCalorieEntryQuery(userId, isAdmin);

        if (from.HasValue)
        {
            query = query.Where(x => x.EntryDate >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.EntryDate <= to.Value);
        }

        var data = await query
            .OrderBy(x => x.EntryDate)
            .Select(x => new
            {
                x.EntryDate,
                x.Calories
            })
            .ToListAsync();

        return data.Select(x => new StatisticsPointDto
        {
            Label = x.EntryDate.ToString("yyyy-MM-dd"),
            Value = x.Calories
        });
    }

    public async Task<IEnumerable<StatisticsPointDto>> GetWorkoutCountsByMonthAsync(string userId, bool isAdmin, int? year = null)
    {
        var query = BuildWorkoutSessionQuery(userId, isAdmin)
            .Where(x => x.Status == WorkoutSessionStatus.Completed);

        if (year.HasValue)
        {
            query = query.Where(x => x.ScheduledAt.Year == year.Value);
        }

        var data = await query
            .Select(x => new
            {
                x.ScheduledAt
            })
            .ToListAsync();

        return data
            .GroupBy(x => new { x.ScheduledAt.Year, x.ScheduledAt.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new StatisticsPointDto
            {
                Label = $"{g.Key.Year}-{g.Key.Month:00}",
                Value = g.Count()
            });
    }

    public async Task<IEnumerable<StatisticsTopExerciseDto>> GetTopExercisesAsync(string userId, bool isAdmin, int take = 5)
    {
        var query = _unitOfWork.WorkoutSessionExercises.Query()
            .AsNoTracking()
            .Include(x => x.WorkoutSession)
            .Include(x => x.Exercise)
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.WorkoutSession.UserId == userId);
        }

        var data = await query
            .Where(x => x.WorkoutSession.Status == WorkoutSessionStatus.Completed)
            .Select(x => new
            {
                x.ExerciseId,
                ExerciseName = x.Exercise.Name
            })
            .ToListAsync();

        return data
            .GroupBy(x => new { x.ExerciseId, x.ExerciseName })
            .Select(g => new StatisticsTopExerciseDto
            {
                ExerciseId = g.Key.ExerciseId,
                ExerciseName = g.Key.ExerciseName,
                Occurrences = g.Count()
            })
            .OrderByDescending(x => x.Occurrences)
            .ThenBy(x => x.ExerciseName)
            .Take(take)
            .ToList();
    }

    public async Task<IEnumerable<StatisticsRecentPersonalRecordDto>> GetRecentPersonalRecordsAsync(string userId, bool isAdmin, int take = 10)
    {
        var query = BuildPersonalRecordQuery(userId, isAdmin)
            .Include(x => x.Exercise)
            .AsQueryable();

        var data = await query
            .OrderByDescending(x => x.RecordDate)
            .ThenByDescending(x => x.CreatedAt)
            .Take(take)
            .Select(x => new StatisticsRecentPersonalRecordDto
            {
                Id = x.Id,
                ExerciseName = x.Exercise.Name,
                RecordDate = x.RecordDate,
                WeightKg = x.WeightKg,
                Reps = x.Reps,
                Notes = x.Notes
            })
            .ToListAsync();

        return data;
    }

    public async Task<AdminStatisticsOverviewDto> GetAdminOverviewAsync()
    {
        return new AdminStatisticsOverviewDto
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalWorkoutPlans = await _unitOfWork.WorkoutPlans.Query().CountAsync(),
            TotalWorkoutSessions = await _unitOfWork.WorkoutSessions.Query().CountAsync(),
            CompletedWorkoutSessions = await _unitOfWork.WorkoutSessions.Query().CountAsync(x => x.Status == WorkoutSessionStatus.Completed),
            TotalMealPlans = await _unitOfWork.MealPlans.Query().CountAsync(),
            TotalExercises = await _unitOfWork.Exercises.Query().CountAsync(),
            TotalFavoriteExercises = await _unitOfWork.FavoriteExercises.Query().CountAsync(),
            TotalBodyMeasurements = await _unitOfWork.BodyMeasurements.Query().CountAsync(),
            TotalCalorieEntries = await _unitOfWork.CalorieEntries.Query().CountAsync(),
            TotalPersonalRecords = await _unitOfWork.PersonalRecords.Query().CountAsync()
        };
    }

    private IQueryable<WorkoutSession> BuildWorkoutSessionQuery(string userId, bool isAdmin)
    {
        var query = _unitOfWork.WorkoutSessions.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return query;
    }

    private IQueryable<BodyMeasurement> BuildBodyMeasurementQuery(string userId, bool isAdmin)
    {
        var query = _unitOfWork.BodyMeasurements.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return query;
    }

    private IQueryable<CalorieEntry> BuildCalorieEntryQuery(string userId, bool isAdmin)
    {
        var query = _unitOfWork.CalorieEntries.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return query;
    }

    private IQueryable<PersonalRecord> BuildPersonalRecordQuery(string userId, bool isAdmin)
    {
        var query = _unitOfWork.PersonalRecords.Query()
            .AsNoTracking()
            .AsQueryable();

        if (!isAdmin)
        {
            query = query.Where(x => x.UserId == userId);
        }

        return query;
    }
}