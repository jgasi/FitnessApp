using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Calories)
            .IsRequired();

        builder.Property(x => x.ProteinGrams)
            .HasPrecision(5, 2);

        builder.Property(x => x.CarbsGrams)
            .HasPrecision(5, 2);

        builder.Property(x => x.FatGrams)
            .HasPrecision(5, 2);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasData(
            new Meal
            {
                Id = 1,
                Name = "Pileća prsa s rižom",
                Description = "Proteinski obrok za ručak ili večeru.",
                Calories = 550,
                ProteinGrams = 45,
                CarbsGrams = 60,
                FatGrams = 12,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Meal
            {
                Id = 2,
                Name = "Zobena kaša s bananom",
                Description = "Dobar doručak za energiju prije treninga.",
                Calories = 380,
                ProteinGrams = 14,
                CarbsGrams = 58,
                FatGrams = 9,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Meal
            {
                Id = 3,
                Name = "Grčki jogurt s voćem",
                Description = "Lagani međuobrok bogat proteinima.",
                Calories = 220,
                ProteinGrams = 18,
                CarbsGrams = 22,
                FatGrams = 6,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Meal
            {
                Id = 4,
                Name = "Tuna salata",
                Description = "Brzi obrok s puno proteina i malo kalorija.",
                Calories = 300,
                ProteinGrams = 28,
                CarbsGrams = 10,
                FatGrams = 14,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}