using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TermApp.Models;

public class AllDbContext : DbContext
{
    public AllDbContext(DbContextOptions<AllDbContext> options) : base(options) { }
    public DbSet<Group> DbGroup { get; set; }
    public DbSet<Note> DbNote { get; set; }
    public DbSet<Term> DbTerm { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Note ↔ Term（1対1、主キー共有）
        modelBuilder.Entity<Note>(entity =>
        {
            entity.ToTable("notes");

            entity.HasKey(n => n.TermId);

            entity.HasOne(n => n.Term)
                  .WithOne(t => t.Note)
                  .HasForeignKey<Note>(n => n.TermId)
                  .OnDelete(DeleteBehavior.Cascade); // Term削除でNoteも削除
        });
    }
}


