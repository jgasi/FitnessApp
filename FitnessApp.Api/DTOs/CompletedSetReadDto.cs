namespace FitnessApp.Api.DTOs;

public class CompletedSetReadDto
{
    public int Id { get; set; }

    public int SetNumber { get; set; }

    public int Reps { get; set; }

    public decimal WeightKg { get; set; }
}