namespace FitnessApp.Api.DTOs;

public class MealPlanReadDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? DailyCaloriesTarget { get; set; }

    public decimal TotalCalories { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<MealPlanMealReadDto> Meals { get; set; } = new();
}