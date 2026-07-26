namespace FitnessApp.Api.DTOs;

public class WorkoutSessionCompleteDto
{
    public List<WorkoutSessionExerciseCompletionDto> Exercises { get; set; } = new();
}