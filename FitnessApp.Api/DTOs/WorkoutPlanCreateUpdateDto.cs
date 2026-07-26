using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutPlanCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public List<WorkoutPlanExerciseCreateUpdateDto> Exercises { get; set; } = new();
}