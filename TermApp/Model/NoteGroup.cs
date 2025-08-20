using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TermApp.Models;

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

    public ICollection<NoteGroup>? Subgroups { get; set; }
    public ICollection<Term>? Terms { get; set; }

}
