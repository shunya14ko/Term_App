using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
//同じ namespace 内のクラスは using しなくても使用可能
//裏でフィールドを自動生成してくれる省略記法
//自動プロパティという機能
//?は型にNULLを許容することを示す
namespace TermApp.Models;

[Index(nameof(ParentId))]
[Table("note_groups")]
public class NoteGroup
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required, MaxLength(255)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("parent_id")]
    public long? ParentId { get; set; }

    [ForeignKey(nameof(ParentId))]
    public NoteGroup? Parent { get; set; }

    // 自己参照コレクション（非nullで初期化）
    [InverseProperty(nameof(Parent))] 　　　// ← 反対側ナビゲーションを明示
    public virtual List<NoteGroup> Subgroups { get; } = new();

    // 用語コレクション（非nullで初期化）
    [InverseProperty(nameof(Term.Group))]
    public virtual List<Term> Terms { get; } = new();

}
