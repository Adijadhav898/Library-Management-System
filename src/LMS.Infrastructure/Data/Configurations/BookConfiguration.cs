using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(500);
        builder.Property(b => b.ISBN).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Language).HasMaxLength(50);
        builder.Property(b => b.Edition).HasMaxLength(50);
        builder.Property(b => b.Description).HasMaxLength(2000);
        builder.Property(b => b.CoverImageUrl).HasMaxLength(500);
        builder.Property(b => b.ShelfLocation).HasMaxLength(100);

        builder.HasIndex(b => b.ISBN).IsUnique();
        builder.HasIndex(b => b.Title);
        builder.HasIndex(b => b.CategoryId);
        builder.HasIndex(b => b.PublisherId);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}