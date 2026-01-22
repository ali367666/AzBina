using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class LandDetailConfiguration : IEntityTypeConfiguration<LandDetails>
{
    public void Configure(EntityTypeBuilder<LandDetails> builder)
    {
        builder.ToTable("LandDetails");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.LandAreaSot)
               .IsRequired();


    }
}
