namespace FitnessApp.Api.DTOs;

public class BodyMeasurementReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime MeasurementDate { get; set; }

    public decimal WeightKg { get; set; }

    public decimal? Bmi { get; set; }

    public decimal? BodyFatPercentage { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}