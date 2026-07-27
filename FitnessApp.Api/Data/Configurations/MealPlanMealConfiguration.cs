using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class MealPlanMealConfiguration : IEntityTypeConfiguration<MealPlanMeal>
{
    public void Configure(EntityTypeBuilder<MealPlanMeal> builder)
    {
        builder.Property(x => x.MealSlot)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        builder.Property(x => x.PortionMultiplier)
            .HasPrecision(5, 2)
            .HasDefaultValue(1m);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasIndex(x => new { x.MealPlanId, x.DisplayOrder })
            .IsUnique();

        builder.HasOne(x => x.MealPlan)
            .WithMany(x => x.MealPlanMeals)
            .HasForeignKey(x => x.MealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Meal)
            .WithMany(x => x.MealPlanMeals)
            .HasForeignKey(x => x.MealId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}