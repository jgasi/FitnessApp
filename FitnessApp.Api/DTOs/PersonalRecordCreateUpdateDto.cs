using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class PersonalRecordCreateUpdateDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ExerciseId { get; set; }

    [Required]
    public DateTime RecordDate { get; set; }

    [Required]
    [Range(typeof(decimal), "0.1", "1000")]
    public decimal WeightKg { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Reps { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}