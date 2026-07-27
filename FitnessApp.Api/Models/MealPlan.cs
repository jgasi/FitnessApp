namespace FitnessApp.Api.Models;

public class MealPlan
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? DailyCaloriesTarget { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MealPlanMeal> MealPlanMeals { get; set; } = new List<MealPlanMeal>();
}