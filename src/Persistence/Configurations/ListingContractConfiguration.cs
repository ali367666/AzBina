using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ListingContactConfiguration : IEntityTypeConfiguration<ListingContact>
{
    public void Configure(EntityTypeBuilder<ListingContact> builder)
    {
        builder.ToTable("ListingContacts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Role)
               .IsRequired();

        builder.Property(x => x.FullName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.Email)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(x => x.PhoneNumber)
               .IsRequired()
               .HasMaxLength(30);

        
        builder.HasOne(x => x.PropertyListing)
               .WithOne(l => l.Contact)                 
               .HasForeignKey<ListingContact>(x => x.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(x => x.PropertyListingId)
               .IsUnique();
    }
}
