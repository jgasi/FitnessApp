using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class MealCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Calories { get; set; }

    [Range(typeof(decimal), "0", "1000")]
    public decimal? ProteinGrams { get; set; }

    [Range(typeof(decimal), "0", "1000")]
    public decimal? CarbsGrams { get; set; }

    [Range(typeof(decimal), "0", "1000")]
    public decimal? FatGrams { get; set; }
}