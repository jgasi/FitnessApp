using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface IProfileService
{
    Task<UserProfileReadDto?> GetMyProfileAsync(string userId);
    Task<bool> UpdateMyProfileAsync(string userId, UserProfileUpdateDto dto);
}