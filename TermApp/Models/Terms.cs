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

[Table("terms")]
public class Term
{
    [Required, Key, Column("id")]
    public long Id { get; set; }

    //Required: NULL禁止
    [Required, MaxLength(255), Column("name")]
    public string Name { get; set; } = string.Empty;

    [Required, Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("group_id")]
    public long? GroupId { get; set; }

    [ForeignKey(nameof(GroupId))]
    public NoteGroup? Group { get; set; }

    public Note? Note { get; set; }
}
