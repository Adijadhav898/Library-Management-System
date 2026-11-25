using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)");
        builder.Property(p => p.TransactionRef).HasMaxLength(100);

        builder.HasIndex(p => p.FineId);
        builder.HasIndex(p => p.PaidAt);

        builder.HasOne(p => p.Fine)
            .WithMany(f => f.Payments)
            .HasForeignKey(p => p.FineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.ReceivedBy)
            .WithMany()
            .HasForeignKey(p => p.ReceivedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}