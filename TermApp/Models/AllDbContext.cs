using Microsoft.EntityFrameworkCore;

namespace TermApp.Models
{
    public class AllDbContext : DbContext
    {
        public AllDbContext(DbContextOptions<AllDbContext> options) : base(options) { }
        public DbSet<NoteGroup> DbNoteGroup { get; set; }
        public DbSet<Note> DbNote { get; set; }
        public DbSet<Term> DbTerm { get; set; }
    }
#warning //複雑なリレーションがあれば記述が必要だがこのレベルでは不要
}

