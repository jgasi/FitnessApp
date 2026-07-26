using FitnessApp.Api.Data.Repositories;
using FitnessApp.Api.Models;

namespace FitnessApp.Api.Data;

public interface IUnitOfWork
{
    IGenericRepository<Exercise> Exercises { get; }
    IGenericRepository<ExerciseCategory> ExerciseCategories { get; }
    IGenericRepository<MuscleGroup> MuscleGroups { get; }
    IGenericRepository<FitnessGoal> FitnessGoals { get; }
    IGenericRepository<UserProfile> UserProfiles { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}