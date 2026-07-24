using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class ExerciseCategoryConfiguration : IEntityTypeConfiguration<ExerciseCategory>
{
    public void Configure(EntityTypeBuilder<ExerciseCategory> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new ExerciseCategory { Id = 1, Name = "Snaga" },
            new ExerciseCategory { Id = 2, Name = "Kardio" },
            new ExerciseCategory { Id = 3, Name = "Istezanje" }
        );
    }
}