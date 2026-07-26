using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IExerciseService
{
    Task<IEnumerable<ExerciseReadDto>> GetAllAsync(string? search = null, int? exerciseCategoryId = null, int? muscleGroupId = null);
    Task<ExerciseReadDto?> GetByIdAsync(int id);
    Task<ExerciseReadDto> CreateAsync(ExerciseCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, ExerciseCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}