using Application.Abstracts.Repositories;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repositories;

public class GenericRepository<TEntity,TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    private readonly BinaDbContext _context;
    DbSet<TEntity> _table;
    public GenericRepository(BinaDbContext context)
    {
        _context = context;
        _table = _context.Set<TEntity>();
    }
    public void Add(TEntity entity)
    {
        _table.Add(entity);
    }

    public List<TEntity> GetAll()
    {
        return _table.ToList();

    }

    public TEntity? GetById(TKey id)
    {
        return _table.Find(id);
    }

   
    public void Delete(TKey id)
    {
        var entity = GetById(id);
        if (entity == null)
            return;

        _table.Remove(entity);
    }

    public void Update(TEntity entity)
    {
        _table.Update(entity);
    }
    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
