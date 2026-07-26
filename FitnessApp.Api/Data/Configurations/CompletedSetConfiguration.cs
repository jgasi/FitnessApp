using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class CompletedSetConfiguration : IEntityTypeConfiguration<CompletedSet>
{
    public void Configure(EntityTypeBuilder<CompletedSet> builder)
    {
        builder.Property(x => x.WeightKg)
            .HasPrecision(6, 2);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.WorkoutSessionExercise)
            .WithMany(x => x.CompletedSets)
            .HasForeignKey(x => x.WorkoutSessionExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}