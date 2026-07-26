namespace FitnessApp.Api.DTOs;

public class WorkoutSessionReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int WorkoutPlanId { get; set; }

    public string WorkoutPlanName { get; set; } = string.Empty;

    public DateTime ScheduledAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<WorkoutSessionExerciseReadDto> Exercises { get; set; } = new();
}