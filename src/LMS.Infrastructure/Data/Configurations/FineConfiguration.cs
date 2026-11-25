using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.ToTable("Fines");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Amount).HasColumnType("decimal(18,2)");
        builder.Property(f => f.Reason).IsRequired().HasMaxLength(200);

        builder.HasIndex(f => f.LoanId);
        builder.HasIndex(f => f.MemberId);
        builder.HasIndex(f => f.Status);

        builder.HasOne(f => f.Loan)
            .WithMany(l => l.Fines)
            .HasForeignKey(f => f.LoanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Member)
            .WithMany(m => m.Fines)
            .HasForeignKey(f => f.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}