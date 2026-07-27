using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class PersonalRecordCreateUpdateDto
{
    [Required]
    public int ExerciseId { get; set; }

    [Required]
    public DateTime RecordDate { get; set; }

    [Required]
    public decimal WeightKg { get; set; }

    [Required]
    public int Reps { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}