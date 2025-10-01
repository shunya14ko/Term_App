using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TermApp.Models;

namespace TermApp.Repositories
{
    public sealed class TermRepository : ICrudRepository<Term>
    {
        private readonly AllDbContext _db;
        public TermRepository(AllDbContext db) => _db = db;

        public async Task<IReadOnlyList<Term>> GetAllAsync(CancellationToken ct = default)
            => await _db.DbTerm.AsNoTracking()
                               .OrderByDescending(t => t.CreatedAt)
                               .ToListAsync(ct);

        public async Task<Term?> GetByIdAsync(long id, CancellationToken ct = default)
            => await _db.DbTerm.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task<long> AddAsync(Term item, CancellationToken ct = default)
        {
            _db.DbTerm.Add(item);
            await _db.SaveChangesAsync(ct);
            return item.Id;
        }

        public async Task<bool> UpdateAsync(Term item, CancellationToken ct = default)
        {
            _db.DbTerm.Update(item);
            return await _db.SaveChangesAsync(ct) > 0;
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            var entity = await _db.DbTerm.FindAsync(new object[] { id }, ct);
            if (entity is null) return false;
            _db.DbTerm.Remove(entity);
            return await _db.SaveChangesAsync(ct) > 0;
        }
    }
}
