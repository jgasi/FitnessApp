using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class WorkoutPlanExerciseConfiguration : IEntityTypeConfiguration<WorkoutPlanExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutPlanExercise> builder)
    {
        builder.HasKey(x => new { x.WorkoutPlanId, x.ExerciseId });

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.Sets)
            .IsRequired();

        builder.Property(x => x.Reps)
            .IsRequired();

        builder.Property(x => x.RestSeconds)
            .IsRequired();

        builder.HasOne(x => x.WorkoutPlan)
            .WithMany(x => x.WorkoutPlanExercises)
            .HasForeignKey(x => x.WorkoutPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}