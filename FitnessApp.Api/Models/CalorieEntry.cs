namespace FitnessApp.Api.Models;

public class CalorieEntry
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public DateTime EntryDate { get; set; }

    public int Calories { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}