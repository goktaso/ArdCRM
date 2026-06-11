using ArdCRM.Core.Entities;
using ArdCRM.Core.Interfaces;
using ArdCRM.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ArdCRM.Data.Repositories;

public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ArdCrmDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ArdCrmDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await _dbSet.FirstOrDefaultAsync(x => x.Id == id && x.Aktif);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> GetAllActiveAsync() =>
        await _dbSet.Where(x => x.Aktif).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        entity.OlusturmaTarihi = DateTime.Now;
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        entity.GuncellemeTarihi = DateTime.Now;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return;
        entity.Aktif = false;
        entity.GuncellemeTarihi = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id) =>
        await _dbSet.AnyAsync(x => x.Id == id && x.Aktif);
}
