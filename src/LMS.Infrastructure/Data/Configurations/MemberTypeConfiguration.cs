using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class MemberTypeConfiguration : IEntityTypeConfiguration<MemberType>
{
    public void Configure(EntityTypeBuilder<MemberType> builder)
    {
        builder.ToTable("MemberTypes");
        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Name).IsRequired().HasMaxLength(50);
        builder.Property(mt => mt.MaxBooksAllowed).HasDefaultValue(3);
        builder.Property(mt => mt.LoanDays).HasDefaultValue(14);
        builder.Property(mt => mt.MaxRenewals).HasDefaultValue(2);
        builder.Property(mt => mt.FinePerDay).HasDefaultValue(1.0m).HasColumnType("decimal(18,2)");
        builder.Property(mt => mt.ReservationExpiryHours).HasDefaultValue(48);

        builder.HasIndex(mt => mt.Name).IsUnique();
    }
}