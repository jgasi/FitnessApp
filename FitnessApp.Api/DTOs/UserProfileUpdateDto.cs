namespace FitnessApp.Api.DTOs;

public class UserProfileUpdateDto
{
    public int? FitnessGoalId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? CurrentWeightKg { get; set; }
}