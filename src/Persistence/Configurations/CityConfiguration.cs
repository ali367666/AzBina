using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable("Cities");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
               .IsRequired()
               .HasMaxLength(100);
        builder.HasMany(x => x.Districts)
                .WithOne(d => d.City)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
