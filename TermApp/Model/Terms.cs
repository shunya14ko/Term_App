using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
//同じ namespace 内のクラスは using しなくても使用可能
//裏でフィールドを自動生成してくれる省略記法
//自動プロパティという機能
//?は型にNULLを許容することを示す
namespace TermApp.Models;

[Table("terms")]
public class Term
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required, MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;　　　// ← string.Empty;はNULLを許容しない場合の初期値

    [Required]
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;　　　// ← 現在のUTC時刻。新規作成時に自動で値をセットして、作成日時を残す

    [Column("group_id")]
    public long? GroupId { get; set; }　　　// ← FKの実カラム（terms.group_id）

    [ForeignKey(nameof(GroupId))]
    public NoteGroup? Group { get; set; }　　　// ← 対応するナビゲーション（親 = note_groups.id）

    public Note? Note { get; set; }　　　// ← これはNotesクラスとの関係（Term:Note = 1:1）
}
