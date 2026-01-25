using Domain.Entities;
using Domain.Entities.Details;
using Microsoft.EntityFrameworkCore;
namespace Persistence.Context;



public class BinaDbContext : DbContext
{
    public BinaDbContext(DbContextOptions<BinaDbContext> options)
        : base(options)
    {
    }

    public DbSet<PropertyListing> PropertyListings { get; set; } = null!;
    public DbSet<MediaProperty> MediaProperties { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<District> Districts { get; set; } = null!;
    public DbSet<ApartmentDetails> ApartmentDetails { get; set; } = null!;
    public DbSet<HouseDetails> HouseDetails { get; set; } = null!;
    public DbSet<LandDetails> LandDetails { get; set; } = null!;
    public DbSet<RentDetails> RentDetails { get; set; } = null!;
    public DbSet<SaleDetails> SaleDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BinaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

