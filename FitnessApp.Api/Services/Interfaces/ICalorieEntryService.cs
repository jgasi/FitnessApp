using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface ICalorieEntryService
{
    Task<IEnumerable<CalorieEntryReadDto>> GetAllAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null);
    Task<CalorieEntryReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<CalorieEntryReadDto> CreateAsync(string userId, CalorieEntryCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, string userId, bool isAdmin, CalorieEntryCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}