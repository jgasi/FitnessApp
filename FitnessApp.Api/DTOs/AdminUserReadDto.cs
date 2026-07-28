namespace FitnessApp.Api.DTOs;

public class AdminUserReadDto
{
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<string> Roles { get; set; } = new();

    public int? FitnessGoalId { get; set; }

    public string? FitnessGoalName { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public decimal? HeightCm { get; set; }

    public decimal? CurrentWeightKg { get; set; }
}