using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class BodyMeasurementCreateUpdateDto
{
    [Required]
    public DateTime MeasurementDate { get; set; }

    [Required]
    public decimal WeightKg { get; set; }

    public decimal? BodyFatPercentage { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}