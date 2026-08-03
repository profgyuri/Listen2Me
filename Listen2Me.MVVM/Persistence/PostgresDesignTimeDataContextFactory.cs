using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Listen2Me.MVVM.Persistence;

/// <summary>
/// Design time data context factory for Postgres. Exists purely for migrations.
/// </summary>
public class PostgresDesignTimeDataContextFactory: IDesignTimeDbContextFactory<PostgresDataContext>
{
    public PostgresDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PostgresDataContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=listen2me_design;Username=postgres;Password=postgres");
        return new PostgresDataContext(optionsBuilder.Options);
    }
}