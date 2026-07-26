using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class WorkoutSessionExerciseConfiguration : IEntityTypeConfiguration<WorkoutSessionExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutSessionExercise> builder)
    {
        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.PlannedSets)
            .IsRequired();

        builder.Property(x => x.PlannedReps)
            .IsRequired();

        builder.Property(x => x.PlannedRestSeconds)
            .IsRequired();

        builder.HasOne(x => x.WorkoutSession)
            .WithMany(x => x.WorkoutSessionExercises)
            .HasForeignKey(x => x.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}