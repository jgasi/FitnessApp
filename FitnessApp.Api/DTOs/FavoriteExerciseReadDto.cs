namespace FitnessApp.Api.DTOs;

public class FavoriteExerciseReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public string ExerciseDescription { get; set; } = string.Empty;

    public string? YoutubeUrl { get; set; }

    public string ExerciseCategoryName { get; set; } = string.Empty;

    public string MuscleGroupName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}