using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence;

/// <summary>
/// Database context for SQLite.
/// </summary>
public class SqLiteDataContext : DbContext
{
    public DbSet<Song> Songs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite(Constants.SqLiteConnectionString);
    }
}