using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class WorkoutSessionExerciseCompletionDto
{
    [Required]
    public int WorkoutSessionExerciseId { get; set; }

    public List<CompletedSetUpsertDto> CompletedSets { get; set; } = new();
}