using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.DueDate).IsRequired();
        builder.Property(l => l.RenewalCount).HasDefaultValue(0);

        builder.HasIndex(l => l.BookCopyId);
        builder.HasIndex(l => l.MemberId);
        builder.HasIndex(l => l.DueDate);
        builder.HasIndex(l => l.Status);

        builder.HasOne(l => l.BookCopy)
            .WithMany(bc => bc.Loans)
            .HasForeignKey(l => l.BookCopyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.IssuedBy)
            .WithMany()
            .HasForeignKey(l => l.IssuedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}