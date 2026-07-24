using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(e => e.YoutubeUrl)
            .HasMaxLength(500);

        builder.HasOne(e => e.ExerciseCategory)
            .WithMany(c => c.Exercises)
            .HasForeignKey(e => e.ExerciseCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.MuscleGroup)
            .WithMany(m => m.Exercises)
            .HasForeignKey(e => e.MuscleGroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}