namespace FitnessApp.Api.DTOs;

public class ExerciseReadDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? YoutubeUrl { get; set; }

    public int ExerciseCategoryId { get; set; }

    public string ExerciseCategoryName { get; set; } = string.Empty;

    public int MuscleGroupId { get; set; }

    public string MuscleGroupName { get; set; } = string.Empty;
}