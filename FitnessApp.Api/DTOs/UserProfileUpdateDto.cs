using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class UserProfileUpdateDto
{
    public int? FitnessGoalId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [Range(typeof(decimal), "0.1", "300")]
    public decimal? HeightCm { get; set; }

    [Range(typeof(decimal), "0.1", "500")]
    public decimal? CurrentWeightKg { get; set; }
}