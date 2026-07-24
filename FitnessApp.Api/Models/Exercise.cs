namespace FitnessApp.Api.Models;

public class Exercise
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? YoutubeUrl { get; set; }

    public int ExerciseCategoryId { get; set; }
    public ExerciseCategory ExerciseCategory { get; set; } = null!;

    public int MuscleGroupId { get; set; }
    public MuscleGroup MuscleGroup { get; set; } = null!;
}