namespace FitnessApp.Api.DTOs;

public class AdminStatisticsOverviewDto
{
    public int TotalUsers { get; set; }

    public int TotalWorkoutPlans { get; set; }

    public int TotalWorkoutSessions { get; set; }

    public int CompletedWorkoutSessions { get; set; }

    public int TotalMealPlans { get; set; }

    public int TotalExercises { get; set; }

    public int TotalFavoriteExercises { get; set; }

    public int TotalBodyMeasurements { get; set; }

    public int TotalCalorieEntries { get; set; }

    public int TotalPersonalRecords { get; set; }
}