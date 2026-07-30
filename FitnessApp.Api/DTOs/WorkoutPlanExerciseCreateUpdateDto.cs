using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutPlanExerciseCreateUpdateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ExerciseId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int DisplayOrder { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Sets { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Reps { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int RestSeconds { get; set; }
}