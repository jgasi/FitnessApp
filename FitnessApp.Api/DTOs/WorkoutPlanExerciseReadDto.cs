namespace FitnessApp.Api.DTOs;

public class WorkoutPlanExerciseReadDto
{
    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public int Sets { get; set; }

    public int Reps { get; set; }

    public int RestSeconds { get; set; }
}