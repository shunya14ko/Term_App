using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TermApp.Dbconn;
using TermApp.Models;
using System.Linq;
using System;

namespace TermApp.Service.Ripository;
public class TermRepository : ICrudRepository<Term>
{
    private readonly AllDbContext _db;
    public TermRepository(AllDbContext db) => _db = db;

    //全件取得
    //AsNoTracking()は読み取り専用で変更しない場合にパフォーマンス向上
    //↑ステータスを保持しないことによって実現
    public async Task<IReadOnlyList<Term>> GetAllAsync(CancellationToken ct = default) =>
        await _db.DbTerm.AsNoTracking().ToListAsync(ct);

    //追加
    public async Task<long> AddAsync(Term item, CancellationToken ct = default)
    {
        _db.DbTerm.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.Id;
    }

    //更新
    //1件以上更新されたかどうか
    public async Task<bool> UpdateAsync(Term item, CancellationToken ct = default)
    {
        _db.DbTerm.Update(item);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //削除
    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await GetByIdAsync(id, ct);
        if (entity is null) return false;
        _db.DbTerm.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    //検索、1件取得（登録、更新、削除に利用）
    public async Task<Term?> GetByIdAsync(long id, CancellationToken ct = default) =>
        await _db.DbTerm.FindAsync(id, ct);

    //検索 部分一致
#warning //呼び出し側でtry-catchで例外処理を行う
    public async Task<IReadOnlyList<Term>> SearchByNameAsync(string keyword, CancellationToken ct = default)
    {
        return await _db.DbTerm
            .Where(t => t.Name.Contains(keyword)
            || t.Notes.Any(n => n.Content.Contains(keyword)))
            .AsNoTracking()
            .ToListAsync(ct);
    }

}
