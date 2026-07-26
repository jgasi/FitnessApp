namespace FitnessApp.Api.DTOs;

public class WorkoutPlanReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<WorkoutPlanExerciseReadDto> Exercises { get; set; } = new();
}