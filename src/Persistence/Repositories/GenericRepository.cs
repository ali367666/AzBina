using Application.Abstracts.Repositories;
using Domain.Entities;
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

    public async Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
    {
        return await _table.ToListAsync(ct);

    }

    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        return _table.FindAsync(new object?[] { id }, ct).AsTask();
    }


    public async Task AddAsync(TEntity entity, CancellationToken ct = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        await _table.AddAsync(entity, ct);
    }

    public void Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _table.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _table.Remove(entity);
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _context.SaveChangesAsync(ct);
    }
}
