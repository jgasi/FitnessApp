using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IMealPlanService
{
    Task<IEnumerable<MealPlanReadDto>> GetAllAsync(string userId, bool isAdmin);
    Task<MealPlanReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<MealPlanReadDto> CreateAsync(string userId, MealPlanCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, string userId, bool isAdmin, MealPlanCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}