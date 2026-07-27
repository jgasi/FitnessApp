namespace FitnessApp.Api.DTOs;

public class CalorieEntryReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }

    public int Calories { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}