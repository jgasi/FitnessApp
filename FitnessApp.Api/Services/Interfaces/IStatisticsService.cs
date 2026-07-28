using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IStatisticsService
{
    Task<StatisticsOverviewDto> GetMyOverviewAsync(string userId, bool isAdmin);
    Task<IEnumerable<StatisticsPointDto>> GetWeightProgressAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<StatisticsPointDto>> GetCaloriesProgressAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<StatisticsPointDto>> GetWorkoutCountsByMonthAsync(string userId, bool isAdmin, int? year = null);
    Task<IEnumerable<StatisticsTopExerciseDto>> GetTopExercisesAsync(string userId, bool isAdmin, int take = 5);
    Task<IEnumerable<StatisticsRecentPersonalRecordDto>> GetRecentPersonalRecordsAsync(string userId, bool isAdmin, int take = 10);
    Task<AdminStatisticsOverviewDto> GetAdminOverviewAsync();
}