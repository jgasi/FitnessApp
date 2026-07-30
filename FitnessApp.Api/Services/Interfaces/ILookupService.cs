using FitnessApp.Api.DTOs;

namespace FitnessApp.Api.Services.Interfaces;

public interface ILookupService
{
    Task<IEnumerable<LookupDto>> GetExerciseCategoriesAsync();
    Task<IEnumerable<LookupDto>> GetMuscleGroupsAsync();
    Task<IEnumerable<LookupDto>> GetFitnessGoalsAsync();
}