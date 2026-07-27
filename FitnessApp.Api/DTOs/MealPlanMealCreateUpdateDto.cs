using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class MealPlanMealCreateUpdateDto
{
    [Required]
    public int MealId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MealSlot { get; set; } = string.Empty;

    [Required]
    public int DisplayOrder { get; set; }

    [Required]
    public decimal PortionMultiplier { get; set; } = 1m;

    [MaxLength(500)]
    public string? Notes { get; set; }
}