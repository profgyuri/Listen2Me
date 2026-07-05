using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listen2Me.MVVM.Persistence;

/// <summary>
/// Database context for Postgres.
/// </summary>
public class PostgresDataContext : DbContext
{
    public DbSet<Song> Songs { get; set; }

    public PostgresDataContext(DbContextOptions<PostgresDataContext> options) : base(options)
    { }
}