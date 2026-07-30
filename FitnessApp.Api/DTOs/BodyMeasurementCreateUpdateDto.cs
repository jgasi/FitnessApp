using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class BodyMeasurementCreateUpdateDto
{
    [Required]
    public DateTime MeasurementDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.1", "500")]
    public decimal WeightKg { get; set; }

    [Range(typeof(decimal), "0", "100")]
    public decimal? BodyFatPercentage { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}