using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class MealPlanCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? DailyCaloriesTarget { get; set; }

    public List<MealPlanMealCreateUpdateDto> Meals { get; set; } = new();
}