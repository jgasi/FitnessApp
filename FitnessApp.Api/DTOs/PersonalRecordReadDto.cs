namespace FitnessApp.Api.DTOs;

public class PersonalRecordReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int ExerciseId { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public string ExerciseCategoryName { get; set; } = string.Empty;

    public string MuscleGroupName { get; set; } = string.Empty;

    public DateTime RecordDate { get; set; }

    public decimal WeightKg { get; set; }

    public int Reps { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}