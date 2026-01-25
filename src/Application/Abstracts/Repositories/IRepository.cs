using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Application.Abstracts.Repositories;

public interface IRepository<TEntity,TKey>where TEntity : BaseEntity<TKey>
{
    List<TEntity> GetAll();
    TEntity? GetById(TKey id);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Delete(TKey id);
    void SaveChanges();
}
