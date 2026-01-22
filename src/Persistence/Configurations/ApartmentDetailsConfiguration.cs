using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ApartmentDetailsConfiguration : IEntityTypeConfiguration<ApartmentDetails>
{
    public void Configure(EntityTypeBuilder<ApartmentDetails> builder)
    {
        builder.ToTable("ApartmentDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Floor)
               .IsRequired();

        builder.Property(x => x.TotalFloors)
                .IsRequired();
        builder.Property(x => x.PropertyListingId)
                .IsRequired();
        builder.HasOne(x => x.PropertyListing)
                .WithOne(p => p.ApartmentDetails)
                .HasForeignKey<ApartmentDetails>(x => x.PropertyListingId)
                .OnDelete(DeleteBehavior.Cascade);


    }
}
