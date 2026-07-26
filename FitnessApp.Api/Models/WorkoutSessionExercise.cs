namespace FitnessApp.Api.Models;

public class WorkoutSessionExercise
{
    public int Id { get; set; }

    public int WorkoutSessionId { get; set; }
    public WorkoutSession WorkoutSession { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public int PlannedSets { get; set; }

    public int PlannedReps { get; set; }

    public int PlannedRestSeconds { get; set; }

    public ICollection<CompletedSet> CompletedSets { get; set; } = new List<CompletedSet>();
}