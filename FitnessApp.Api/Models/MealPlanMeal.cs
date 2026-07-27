namespace FitnessApp.Api.Models;

public class MealPlanMeal
{
    public int Id { get; set; }

    public int MealPlanId { get; set; }
    public MealPlan MealPlan { get; set; } = null!;

    public int MealId { get; set; }
    public Meal Meal { get; set; } = null!;

    public string MealSlot { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public decimal PortionMultiplier { get; set; } = 1m;

    public string? Notes { get; set; }
}