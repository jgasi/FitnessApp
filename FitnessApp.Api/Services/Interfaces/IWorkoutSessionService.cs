using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IWorkoutSessionService
{
    Task<IEnumerable<WorkoutSessionReadDto>> GetAllAsync(string userId, bool isAdmin);
    Task<WorkoutSessionReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<WorkoutSessionReadDto> CreateAsync(string userId, bool isAdmin, WorkoutSessionCreateDto dto);
    Task<bool> UpdateStatusAsync(int id, string userId, bool isAdmin, WorkoutSessionUpdateStatusDto dto);
    Task<bool> CompleteAsync(int id, string userId, bool isAdmin, WorkoutSessionCompleteDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}