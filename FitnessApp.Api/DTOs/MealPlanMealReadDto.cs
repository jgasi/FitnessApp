namespace FitnessApp.Api.DTOs;

public class MealPlanMealReadDto
{
    public int Id { get; set; }

    public int MealId { get; set; }

    public string MealName { get; set; } = string.Empty;

    public int MealCalories { get; set; }

    public decimal? ProteinGrams { get; set; }

    public decimal? CarbsGrams { get; set; }

    public decimal? FatGrams { get; set; }

    public string MealSlot { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public decimal PortionMultiplier { get; set; }

    public string? Notes { get; set; }
}