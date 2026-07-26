using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Models;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<UserProfileReadDto?> GetMyProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        var profile = await _unitOfWork.UserProfiles.Query()
            .AsNoTracking()
            .Include(x => x.FitnessGoal)
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return null;
        }

        return new UserProfileReadDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            FitnessGoalId = profile.FitnessGoalId,
            FitnessGoalName = profile.FitnessGoal?.Name,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            HeightCm = profile.HeightCm,
            CurrentWeightKg = profile.CurrentWeightKg
        };
    }

    public async Task<bool> UpdateMyProfileAsync(string userId, UserProfileUpdateDto dto)
    {
        var profile = await _unitOfWork.UserProfiles.Query()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (profile == null)
        {
            return false;
        }

        if (dto.FitnessGoalId.HasValue)
        {
            var goalExists = await _unitOfWork.FitnessGoals.Query().AnyAsync(x => x.Id == dto.FitnessGoalId.Value);
            if (!goalExists)
            {
                throw new ArgumentException("Fitness cilj ne postoji.");
            }
        }

        profile.FitnessGoalId = dto.FitnessGoalId;
        profile.DateOfBirth = dto.DateOfBirth;
        profile.Gender = dto.Gender;
        profile.HeightCm = dto.HeightCm;
        profile.CurrentWeightKg = dto.CurrentWeightKg;

        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}