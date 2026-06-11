using ArdCRM.Core.Entities;

namespace ArdCRM.Core.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllActiveAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);   // soft delete — Aktif = false
    Task<bool> ExistsAsync(int id);
}
