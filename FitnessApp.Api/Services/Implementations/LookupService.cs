using FitnessApp.Api.Data;
using FitnessApp.Api.DTOs;
using FitnessApp.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Services.Implementations;

public class LookupService : ILookupService
{
    private readonly IUnitOfWork _unitOfWork;

    public LookupService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<LookupDto>> GetExerciseCategoriesAsync()
    {
        return await _unitOfWork.ExerciseCategories.Query()
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<LookupDto>> GetMuscleGroupsAsync()
    {
        return await _unitOfWork.MuscleGroups.Query()
            .AsNoTracking()
            .Select(x => new LookupDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();
    }
}