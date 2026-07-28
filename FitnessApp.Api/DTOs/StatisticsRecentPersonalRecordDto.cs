namespace FitnessApp.Api.DTOs;

public class StatisticsRecentPersonalRecordDto
{
    public int Id { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public DateTime RecordDate { get; set; }

    public decimal WeightKg { get; set; }

    public int Reps { get; set; }

    public string? Notes { get; set; }
}