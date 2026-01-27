using Application.Abstracts.Repositories;
using Domain.Entities;
using Persistence.Context;

namespace Persistence.Repositories;

public class DistrictRepository:GenericRepository<District,int>,IDistrictRepository
{
    public DistrictRepository(BinaDbContext context) : base(context)
    {
    }
}
