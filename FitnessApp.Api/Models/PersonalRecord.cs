namespace FitnessApp.Api.Models;

public class PersonalRecord
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public DateTime RecordDate { get; set; }

    public decimal WeightKg { get; set; }

    public int Reps { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}