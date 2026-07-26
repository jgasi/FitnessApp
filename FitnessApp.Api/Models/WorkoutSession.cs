namespace FitnessApp.Api.Models;

public class WorkoutSession
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;

    public DateTime ScheduledAt { get; set; }

    public WorkoutSessionStatus Status { get; set; } = WorkoutSessionStatus.Planned;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WorkoutSessionExercise> WorkoutSessionExercises { get; set; } = new List<WorkoutSessionExercise>();
}