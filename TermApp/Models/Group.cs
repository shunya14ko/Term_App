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
//InverseProperty： 逆ナビゲーションプロパティを指定

namespace TermApp.Models;

[Index(nameof(ParentId))]
[Table("note_groups")]
public class Group
{
    [Key, Column("id")]
    public long Id { get; set; }

    [Required, MaxLength(255), Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("parent_id")]
    public long? ParentId { get; set; }

    [InverseProperty(nameof(Term.Group))]
    public virtual List<Term> Terms { get; } = new();

}
