namespace FitnessApp.Api.Models;

public class FavoriteExercise
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}