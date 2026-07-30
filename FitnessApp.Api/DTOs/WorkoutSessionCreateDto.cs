using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutSessionCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int WorkoutPlanId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}