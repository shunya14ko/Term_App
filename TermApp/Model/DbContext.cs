using Microsoft.EntityFrameworkCore;
using TermApp.Models;

namespace TermApp.Model
{
    public class AllDbContext : DbContext
    {
        public AllDbContext(DbContextOptions<DbContext> options) : base(options)
        { }
        public DbSet<NoteGroup> DbNoteGroup { get; set; }
        public DbSet<Note> DbNote { get; set; }
        public DbSet<Term> DbTerm { get; set; }

        // Additional code ...
    }
}
