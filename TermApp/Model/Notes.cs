using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//同じ namespace 内のクラスは using しなくても使用可能

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

    // ナビゲーション（カラムじゃない）
    [ForeignKey(nameof(TermId))]
    public Terms Term { get; set; } = null!;

    [Column("content")]
    public string? Content { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
