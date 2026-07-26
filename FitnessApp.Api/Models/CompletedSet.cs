namespace FitnessApp.Api.Models;

public class CompletedSet
{
    public int Id { get; set; }

    public int WorkoutSessionExerciseId { get; set; }
    public WorkoutSessionExercise WorkoutSessionExercise { get; set; } = null!;

    public int SetNumber { get; set; }

    public int Reps { get; set; }

    public decimal WeightKg { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}