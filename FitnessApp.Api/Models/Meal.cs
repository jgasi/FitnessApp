namespace FitnessApp.Api.Models;

public class Meal
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Calories { get; set; }

    public decimal? ProteinGrams { get; set; }

    public decimal? CarbsGrams { get; set; }

    public decimal? FatGrams { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MealPlanMeal> MealPlanMeals { get; set; } = new List<MealPlanMeal>();
}