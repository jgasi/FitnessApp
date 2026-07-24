namespace FitnessApp.Api.Models;

public class UserProfile
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int? FitnessGoalId { get; set; }
    public FitnessGoal? FitnessGoal { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? CurrentWeightKg { get; set; }
}