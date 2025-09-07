using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

//裏でフィールドを自動生成してくれる省略記法
//自動プロパティという機能
//?は型にNULLを許容することを示す
//Key：主キー
//Column：DBのカラム名
//Required: NULL禁止
//Column属性を使用して、データベースの列名を指定
//Table属性を使用して、データベースのテーブル名を指定

namespace TermApp.Models;

[Table("notes")]
public class Note
{
    [Key, ForeignKey(nameof(Term)), Column("term_id")]
    public long TermId { get; set; }

    public Term Term { get; set; } = null!;

    [Column("content")]
    public string? Content { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }
}
