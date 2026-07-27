using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IBodyMeasurementService
{
    Task<IEnumerable<BodyMeasurementReadDto>> GetAllAsync(string userId, bool isAdmin, DateTime? from = null, DateTime? to = null);
    Task<BodyMeasurementReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<BodyMeasurementReadDto> CreateAsync(string userId, BodyMeasurementCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, string userId, bool isAdmin, BodyMeasurementCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}