using Application.Abstracts.Repositories;
using Domain.Entities;
using Persistence.Context;

namespace Persistence.Repositories;

public class PropertyListeningRepository: GenericRepository<PropertyListing, int>,IPropertyListeningRepository
{
    public PropertyListeningRepository(BinaDbContext context) : base(context)
    {
    }
}
