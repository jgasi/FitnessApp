namespace FitnessApp.Api.DTOs;

public class WorkoutSessionExerciseReadDto
{
    public int Id { get; set; }

    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public int PlannedSets { get; set; }

    public int PlannedReps { get; set; }

    public int PlannedRestSeconds { get; set; }

    public List<CompletedSetReadDto> CompletedSets { get; set; } = new();
}