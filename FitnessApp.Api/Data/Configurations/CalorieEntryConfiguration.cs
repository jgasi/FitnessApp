using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class CalorieEntryConfiguration : IEntityTypeConfiguration<CalorieEntry>
{
    public void Configure(EntityTypeBuilder<CalorieEntry> builder)
    {
        builder.Property(x => x.EntryDate)
            .IsRequired();

        builder.Property(x => x.Calories)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.EntryDate })
            .IsUnique();
    }
}