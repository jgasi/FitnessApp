using FitnessApp.Api.Data.Repositories;
using FitnessApp.Api.Models;

namespace FitnessApp.Api.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private IGenericRepository<Exercise>? _exercises;
    private IGenericRepository<ExerciseCategory>? _exerciseCategories;
    private IGenericRepository<MuscleGroup>? _muscleGroups;
    private IGenericRepository<FitnessGoal>? _fitnessGoals;
    private IGenericRepository<UserProfile>? _userProfiles;
    private IGenericRepository<WorkoutPlan>? _workoutPlans;
    private IGenericRepository<WorkoutPlanExercise>? _workoutPlanExercises;
    private IGenericRepository<WorkoutSession>? _workoutSessions;
    private IGenericRepository<WorkoutSessionExercise>? _workoutSessionExercises;
    private IGenericRepository<CompletedSet>? _completedSets;
    private IGenericRepository<BodyMeasurement>? _bodyMeasurements;
    private IGenericRepository<CalorieEntry>? _calorieEntries;
    private IGenericRepository<FavoriteExercise>? _favoriteExercises;
    private IGenericRepository<PersonalRecord>? _personalRecords;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Exercise> Exercises => _exercises ??= new GenericRepository<Exercise>(_context);
    public IGenericRepository<ExerciseCategory> ExerciseCategories => _exerciseCategories ??= new GenericRepository<ExerciseCategory>(_context);
    public IGenericRepository<MuscleGroup> MuscleGroups => _muscleGroups ??= new GenericRepository<MuscleGroup>(_context);
    public IGenericRepository<FitnessGoal> FitnessGoals => _fitnessGoals ??= new GenericRepository<FitnessGoal>(_context);
    public IGenericRepository<UserProfile> UserProfiles => _userProfiles ??= new GenericRepository<UserProfile>(_context);
    public IGenericRepository<WorkoutPlan> WorkoutPlans => _workoutPlans ??= new GenericRepository<WorkoutPlan>(_context);
    public IGenericRepository<WorkoutPlanExercise> WorkoutPlanExercises => _workoutPlanExercises ??= new GenericRepository<WorkoutPlanExercise>(_context);
    public IGenericRepository<WorkoutSession> WorkoutSessions => _workoutSessions ??= new GenericRepository<WorkoutSession>(_context);
    public IGenericRepository<WorkoutSessionExercise> WorkoutSessionExercises => _workoutSessionExercises ??= new GenericRepository<WorkoutSessionExercise>(_context);
    public IGenericRepository<CompletedSet> CompletedSets => _completedSets ??= new GenericRepository<CompletedSet>(_context);
    public IGenericRepository<BodyMeasurement> BodyMeasurements => _bodyMeasurements ??= new GenericRepository<BodyMeasurement>(_context);
    public IGenericRepository<CalorieEntry> CalorieEntries => _calorieEntries ??= new GenericRepository<CalorieEntry>(_context);
    public IGenericRepository<FavoriteExercise> FavoriteExercises => _favoriteExercises ??= new GenericRepository<FavoriteExercise>(_context);
    public IGenericRepository<PersonalRecord> PersonalRecords => _personalRecords ??= new GenericRepository<PersonalRecord>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}