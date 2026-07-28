namespace FitnessApp.Api.DTOs;

public class StatisticsTopExerciseDto
{
    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public int Occurrences { get; set; }
}