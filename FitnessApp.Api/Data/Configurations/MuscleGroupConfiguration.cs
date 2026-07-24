using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class MuscleGroupConfiguration : IEntityTypeConfiguration<MuscleGroup>
{
    public void Configure(EntityTypeBuilder<MuscleGroup> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new MuscleGroup { Id = 1, Name = "Prsa" },
            new MuscleGroup { Id = 2, Name = "Leđa" },
            new MuscleGroup { Id = 3, Name = "Ramena" },
            new MuscleGroup { Id = 4, Name = "Biceps" },
            new MuscleGroup { Id = 5, Name = "Triceps" },
            new MuscleGroup { Id = 6, Name = "Noge" },
            new MuscleGroup { Id = 7, Name = "Trbuh" },
            new MuscleGroup { Id = 8, Name = "Cijelo tijelo" }
        );
    }
}