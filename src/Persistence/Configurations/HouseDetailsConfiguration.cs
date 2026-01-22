using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class HouseDetailsConfiguration : IEntityTypeConfiguration<HouseDetails>
{
    public void Configure(EntityTypeBuilder<HouseDetails> builder)
    {
        builder.ToTable("HouseDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.HouseFloors)
               .IsRequired();

        builder.Property(x => x.LandAreaSot)
                .IsRequired();

        builder.HasOne(x => x.PropertyListing)
               .WithOne(p => p.HouseDetails)
               .HasForeignKey<HouseDetails>(x => x.PropertyListingId)
               .OnDelete(DeleteBehavior.Cascade);

    }
}
