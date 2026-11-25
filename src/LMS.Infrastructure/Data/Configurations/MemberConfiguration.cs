using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Entities;

namespace LMS.Infrastructure.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MembershipNumber).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Address).HasMaxLength(500);
        builder.Property(m => m.BlockReason).HasMaxLength(500);

        builder.HasIndex(m => m.MembershipNumber).IsUnique();
        builder.HasIndex(m => m.UserId).IsUnique();
        builder.HasIndex(m => m.MemberTypeId);

        builder.HasOne(m => m.MemberType)
            .WithMany(mt => mt.Members)
            .HasForeignKey(m => m.MemberTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}