using Microsoft.EntityFrameworkCore;

namespace Persistence.Configurations;

using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PropertyListingConfiguration : IEntityTypeConfiguration<PropertyListing>
{
    public void Configure(EntityTypeBuilder<PropertyListing> builder)
    {
        builder.ToTable("PropertyListings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(x => x.Description)
               .IsRequired()
               .HasMaxLength(3000);

        builder.Property(x => x.ListingType)
               .IsRequired();

        builder.Property(x => x.PropertyType)
               .IsRequired();

        builder.Property(x => x.Area)
               .IsRequired();

        builder.Property(x => x.Rooms)
               .IsRequired();

        builder.Property(x => x.RenovationStatus)
               .IsRequired();

    
        builder.HasOne(x => x.City)
               .WithMany()
               .HasForeignKey(x => x.CityId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.District)
               .WithMany()
               .HasForeignKey(x => x.DistrictId)
               .OnDelete(DeleteBehavior.Restrict);

 


        builder.HasOne(x => x.Contact)
               .WithOne(c => c.PropertyListing)
               .HasForeignKey<ListingContact>(c => c.PropertyListingId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.SaleDetails)
               .WithOne(sd => sd.PropertyListing)
               .HasForeignKey<SaleDetails>(sd => sd.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RentDetails)
               .WithOne(rd => rd.PropertyListing)
               .HasForeignKey<RentDetails>(rd => rd.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ApartmentDetails)
               .WithOne(ad => ad.PropertyListing)
               .HasForeignKey<ApartmentDetails>(ad => ad.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.HouseDetails)
               .WithOne(hd => hd.PropertyListing)
               .HasForeignKey<HouseDetails>(hd => hd.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LandDetails)
               .WithOne(ld => ld.PropertyListing)
               .HasForeignKey<LandDetails>(ld => ld.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
