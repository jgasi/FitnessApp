using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IFavoriteExerciseService
{
    Task<IEnumerable<FavoriteExerciseReadDto>> GetAllAsync(string userId, bool isAdmin);
    Task<FavoriteExerciseReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<FavoriteExerciseReadDto> CreateAsync(string userId, FavoriteExerciseCreateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}