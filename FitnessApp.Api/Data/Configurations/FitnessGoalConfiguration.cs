using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class FitnessGoalConfiguration : IEntityTypeConfiguration<FitnessGoal>
{
    public void Configure(EntityTypeBuilder<FitnessGoal> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new FitnessGoal { Id = 1, Name = "Mršavljenje" },
            new FitnessGoal { Id = 2, Name = "Povećanje mišićne mase" },
            new FitnessGoal { Id = 3, Name = "Održavanje forme" }
        );
    }
}