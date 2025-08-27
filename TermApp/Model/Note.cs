using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
//同じ namespace 内のクラスは using しなくても使用可能
//裏でフィールドを自動生成してくれる省略記法
//自動プロパティという機能
//?は型にNULLを許容することを示す
namespace TermApp.Models;

[Table("notes")]
public class Note
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("term_id")]
    public long TermId { get; set; }

    [ForeignKey(nameof(TermId))]　　　// ← ナビゲーション（カラムじゃない）
    public Term Term { get; set; } = null!;

    [Column("content")]
    public string? Content { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
