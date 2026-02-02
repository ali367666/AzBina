using Domain.Entities;

namespace Application.Abstracts.Repositories;

public interface IMediaPropertyRepository:IRepository<MediaProperty,int>
{
    Task<int> CountByPropertyListingIdAsync(int propertyListingId, CancellationToken ct = default);
    Task<int> GetMaxOrderByPropertyListingIdAsync(int propertyListingId, CancellationToken ct = default);
    
}
