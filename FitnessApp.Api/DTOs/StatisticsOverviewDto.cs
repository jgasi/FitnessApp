namespace FitnessApp.Api.DTOs;

public class StatisticsOverviewDto
{
    public int TotalWorkoutSessions { get; set; }

    public int CompletedWorkoutSessions { get; set; }

    public int TotalBodyMeasurements { get; set; }

    public int TotalCalorieEntries { get; set; }

    public int TotalPersonalRecords { get; set; }

    public decimal? StartWeightKg { get; set; }

    public decimal? CurrentWeightKg { get; set; }

    public decimal? WeightChangeKg { get; set; }

    public decimal? AverageCalories { get; set; }
}