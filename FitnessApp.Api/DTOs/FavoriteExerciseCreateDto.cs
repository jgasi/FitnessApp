using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class FavoriteExerciseCreateDto
{
    [Required]
    public int ExerciseId { get; set; }
}