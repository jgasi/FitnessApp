using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class MealPlanMealCreateUpdateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int MealId { get; set; }

    [Required]
    [MaxLength(100)]
    public string MealSlot { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [Range(typeof(decimal), "0.1", "100")]
    public decimal PortionMultiplier { get; set; } = 1m;

    [MaxLength(500)]
    public string? Notes { get; set; }
}