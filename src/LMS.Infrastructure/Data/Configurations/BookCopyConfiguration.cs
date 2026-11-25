using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Barcode).IsRequired().HasMaxLength(50);
        builder.Property(bc => bc.Condition).HasMaxLength(200);
        builder.Property(bc => bc.Price).HasColumnType("decimal(18,2)");

        builder.HasIndex(bc => bc.Barcode).IsUnique();
        builder.HasIndex(bc => bc.BookId);
        builder.HasIndex(bc => bc.Status);

        builder.HasOne(bc => bc.Book)
            .WithMany(b => b.BookCopies)
            .HasForeignKey(bc => bc.BookId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}