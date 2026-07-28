using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class AdminUserStatusUpdateDto
{
    [Required]
    public bool IsActive { get; set; }
}