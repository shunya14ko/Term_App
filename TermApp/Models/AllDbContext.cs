using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TermApp.Models;

public class AllDbContext : DbContext
{
    public AllDbContext(DbContextOptions<AllDbContext> options) : base(options) { }
    public DbSet<Group> DbGroup { get; set; }
    public DbSet<Note> DbNote { get; set; }
    public DbSet<Term> DbTerm { get; set; }
}


