using Microsoft.EntityFrameworkCore;
using TermApp.Models;

namespace TermApp.Service.Repository

{
    public interface ICrudRepository<T>
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        Task<T?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<long> AddAsync(T item, CancellationToken ct = default);
        Task<bool> UpdateAsync(T item, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }

}
