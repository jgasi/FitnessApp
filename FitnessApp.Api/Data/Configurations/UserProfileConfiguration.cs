using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasIndex(x => x.UserId).IsUnique();

        builder.Property(x => x.Gender)
            .HasMaxLength(20);

        builder.Property(x => x.HeightCm)
            .HasPrecision(5, 2);

        builder.Property(x => x.CurrentWeightKg)
            .HasPrecision(5, 2);

        builder.HasOne(x => x.User)
            .WithOne(x => x.UserProfile)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FitnessGoal)
            .WithMany(x => x.UserProfiles)
            .HasForeignKey(x => x.FitnessGoalId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}