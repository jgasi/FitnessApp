using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IWorkoutPlanService
{
    Task<IEnumerable<WorkoutPlanReadDto>> GetAllAsync(string userId, bool isAdmin);
    Task<WorkoutPlanReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<WorkoutPlanReadDto> CreateAsync(string userId, WorkoutPlanCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, string userId, bool isAdmin, WorkoutPlanCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}