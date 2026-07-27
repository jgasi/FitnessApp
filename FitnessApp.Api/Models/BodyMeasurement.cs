namespace FitnessApp.Api.Models;

public class BodyMeasurement
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime MeasurementDate { get; set; }

    public decimal WeightKg { get; set; }

    public decimal? Bmi { get; set; }

    public decimal? BodyFatPercentage { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}