using FitnessApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessApp.Api.Data.Configurations;

public class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> builder)
    {
        builder.Property(x => x.MeasurementDate)
            .IsRequired();

        builder.Property(x => x.WeightKg)
            .HasPrecision(6, 2)
            .IsRequired();

        builder.Property(x => x.Bmi)
            .HasPrecision(6, 2);

        builder.Property(x => x.BodyFatPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.MeasurementDate });
    }
}