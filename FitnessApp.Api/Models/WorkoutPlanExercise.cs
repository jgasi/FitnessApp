namespace FitnessApp.Api.Models;

public class WorkoutPlanExercise
{
    public int WorkoutPlanId { get; set; }
    public WorkoutPlan WorkoutPlan { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int DisplayOrder { get; set; }

    public int Sets { get; set; }

    public int Reps { get; set; }

    public int RestSeconds { get; set; }
}