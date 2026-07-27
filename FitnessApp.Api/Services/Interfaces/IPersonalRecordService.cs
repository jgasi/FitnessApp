using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IPersonalRecordService
{
    Task<IEnumerable<PersonalRecordReadDto>> GetAllAsync(string userId, bool isAdmin, int? exerciseId = null, DateTime? from = null, DateTime? to = null);
    Task<PersonalRecordReadDto?> GetByIdAsync(int id, string userId, bool isAdmin);
    Task<PersonalRecordReadDto> CreateAsync(string userId, PersonalRecordCreateUpdateDto dto);
    Task<bool> UpdateAsync(int id, string userId, bool isAdmin, PersonalRecordCreateUpdateDto dto);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
}