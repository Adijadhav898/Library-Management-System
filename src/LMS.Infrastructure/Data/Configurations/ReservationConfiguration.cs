using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.ExpiresAt).IsRequired();
        builder.Property(r => r.Notes).HasMaxLength(500);

        builder.HasIndex(r => r.BookId);
        builder.HasIndex(r => r.MemberId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.ExpiresAt);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Reservations)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Member)
            .WithMany(m => m.Reservations)
            .HasForeignKey(r => r.MemberId)
            .OnDelete(DeleteBehavior.Cascade);
        

        builder.HasOne(r => r.BookCopy)
            .WithMany()
            .HasForeignKey(r => r.BookCopyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}