using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Api.DTOs;

public class AdminUserRoleUpdateDto
{
    [Required]
    public string Role { get; set; } = string.Empty;
}