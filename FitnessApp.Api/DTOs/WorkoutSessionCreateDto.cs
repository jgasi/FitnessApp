using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutSessionCreateDto
{
    [Required]
    public int WorkoutPlanId { get; set; }

    [Required]
    public DateTime ScheduledAt { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}