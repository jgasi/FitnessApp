using FitnessApp.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessApp.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<ExerciseCategory> ExerciseCategories { get; set; }
    public DbSet<MuscleGroup> MuscleGroups { get; set; }
    public DbSet<Exercise> Exercises { get; set; }

    public DbSet<FitnessGoal> FitnessGoals { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
    public DbSet<WorkoutPlanExercise> WorkoutPlanExercises { get; set; }

    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<WorkoutSessionExercise> WorkoutSessionExercises { get; set; }
    public DbSet<CompletedSet> CompletedSets { get; set; }

    public DbSet<BodyMeasurement> BodyMeasurements { get; set; }
    public DbSet<CalorieEntry> CalorieEntries { get; set; }
    public DbSet<FavoriteExercise> FavoriteExercises { get; set; }
    public DbSet<PersonalRecord> PersonalRecords { get; set; }

    public DbSet<Meal> Meals { get; set; }
    public DbSet<MealPlan> MealPlans { get; set; }
    public DbSet<MealPlanMeal> MealPlanMeals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(x => x.FirstName)
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .HasMaxLength(100);

            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.Property(x => x.IsActive)
                .HasDefaultValue(true);
        });

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}