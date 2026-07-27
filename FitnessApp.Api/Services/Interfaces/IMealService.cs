using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IMealService
{
    Task<IEnumerable<MealReadDto>> GetAllAsync();
    Task<MealReadDto?> GetByIdAsync(int id);
    Task<MealReadDto> CreateAsync(MealCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, MealCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}