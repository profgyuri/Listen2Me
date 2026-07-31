using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Listen2Me.MVVM.Persistence;

/// <summary>
/// Defines a shared database context.
/// </summary>
public interface ISharedDbContext
{
    public DbSet<Song> Songs { get; }
    
    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(CancellationToken)"/>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    
    /// <inheritdoc cref="Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync(DatabaseFacade, CancellationToken)"/>
    Task MigrateAsync(CancellationToken ct = default);
}