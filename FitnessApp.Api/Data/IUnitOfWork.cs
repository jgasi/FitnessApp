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
    IGenericRepository<WorkoutPlan> WorkoutPlans { get; }
    IGenericRepository<WorkoutPlanExercise> WorkoutPlanExercises { get; }
    IGenericRepository<WorkoutSession> WorkoutSessions { get; }
    IGenericRepository<WorkoutSessionExercise> WorkoutSessionExercises { get; }
    IGenericRepository<CompletedSet> CompletedSets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}