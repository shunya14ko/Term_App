using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TermApp.Dbconn;
using TermApp.Models;

namespace TermApp.Service.Ripository;

    public sealed class GroupRepository : ICrudRepository<Group>
{
    private readonly AllDbContext _db;
    public GroupRepository(AllDbContext db) => _db = db;

    //全件取得
    //AsNoTracking()は読み取り専用で変更しない場合にパフォーマンス向上
    //↑ステータスを保持しないことによって実現
    public async Task<IReadOnlyList<Group>> GetAllAsync(CancellationToken ct = default) =>
        await _db.DbGroup.AsNoTracking().ToListAsync(ct);

    //追加
    public async Task<long> AddAsync(Group item, CancellationToken ct = default)
    {
        _db.DbGroup.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.Id;
    }

    //更新
    public async Task<bool> UpdateAsync(Group item, CancellationToken ct = default)
    {
        _db.DbGroup.Update(item);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //削除
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return false;
        _db.DbGroup.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //検索
    public async Task<Group?> GetByIdAsync(long id, CancellationToken ct = default) =>
        await _db.DbGroup.FindAsync(id, ct);
}
