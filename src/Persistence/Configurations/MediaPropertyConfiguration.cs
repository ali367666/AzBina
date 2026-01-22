using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MediaPropertyConfiguration : IEntityTypeConfiguration<MediaProperty>
{
    public void Configure(EntityTypeBuilder<MediaProperty> builder)
    {
        builder.ToTable("MediaProperties");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MediaUrl)
               .IsRequired()
               .HasMaxLength(1000);

        builder.Property(x => x.MediaType)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(x => x.Order)
               .IsRequired();

        builder.HasOne(x => x.PropertyListing)
               .WithMany(l => l.MediaProperties)
               .HasForeignKey(x => x.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade); // istəyirsənsə NoAction da edə bilərsən

        builder.HasIndex(x => x.PropertyListingId);
        builder.HasIndex(x => new { x.PropertyListingId, x.Order }).IsUnique();
    }
}
