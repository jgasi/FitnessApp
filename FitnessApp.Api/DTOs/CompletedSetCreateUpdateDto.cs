using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class CompletedSetUpsertDto
{
    [Required]
    public int SetNumber { get; set; }

    [Required]
    public int Reps { get; set; }

    [Required]
    public decimal WeightKg { get; set; }
}