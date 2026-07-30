using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class FavoriteExerciseCreateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ExerciseId { get; set; }
}