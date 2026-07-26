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

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Exercise> Exercises => _exercises ??= new GenericRepository<Exercise>(_context);
    public IGenericRepository<ExerciseCategory> ExerciseCategories => _exerciseCategories ??= new GenericRepository<ExerciseCategory>(_context);
    public IGenericRepository<MuscleGroup> MuscleGroups => _muscleGroups ??= new GenericRepository<MuscleGroup>(_context);
    public IGenericRepository<FitnessGoal> FitnessGoals => _fitnessGoals ??= new GenericRepository<FitnessGoal>(_context);
    public IGenericRepository<UserProfile> UserProfiles => _userProfiles ??= new GenericRepository<UserProfile>(_context);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}