using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutPlanExerciseCreateUpdateDto
{
    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public int DisplayOrder { get; set; }

    [Required]
    public int Sets { get; set; }

    [Required]
    public int Reps { get; set; }

    [Required]
    public int RestSeconds { get; set; }
}