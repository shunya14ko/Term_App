using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TermApp.Dbconn;
using TermApp.Models;

namespace TermApp.Service.Ripository;

public class NoteRepository : ICrudRepository<Note>
{
    private readonly AllDbContext _db;
    public NoteRepository(AllDbContext db) => _db = db;

    //全件取得
    //AsNoTracking()は読み取り専用で変更しない場合にパフォーマンス向上
    //↑ステータスを保持しないことによって実現
    public async Task<IReadOnlyList<Note>> GetAllAsync(CancellationToken ct = default) =>
        await _db.DbNote.AsNoTracking().ToListAsync(ct);

    //追加
    public async Task<long> AddAsync(Note item, CancellationToken ct = default)
    {
        _db.DbNote.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.TermId;
    }

    //更新
    public async Task<bool> UpdateAsync(Note item, CancellationToken ct = default)
    {
        _db.DbNote.Update(item);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //削除
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return false;
        _db.DbNote.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //検索機能を持たないので、未実装
    public Task<Note?> GetByIdAsync(long id, CancellationToken ct = default)
    => throw new NotImplementedException();
}