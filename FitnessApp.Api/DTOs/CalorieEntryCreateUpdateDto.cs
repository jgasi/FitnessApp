using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class CalorieEntryCreateUpdateDto
{
    [Required]
    public DateTime EntryDate { get; set; }

    [Required]
    public int Calories { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}