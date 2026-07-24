using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class ExerciseCreateUpdateDto
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? YoutubeUrl { get; set; }

    [Required]
    public int ExerciseCategoryId { get; set; }

    [Required]
    public int MuscleGroupId { get; set; }
}