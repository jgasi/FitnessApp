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
    IGenericRepository<BodyMeasurement> BodyMeasurements { get; }
    IGenericRepository<CalorieEntry> CalorieEntries { get; }
    IGenericRepository<FavoriteExercise> FavoriteExercises { get; }
    IGenericRepository<PersonalRecord> PersonalRecords { get; }
    IGenericRepository<Meal> Meals { get; }
    IGenericRepository<MealPlan> MealPlans { get; }
    IGenericRepository<MealPlanMeal> MealPlanMeals { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}