using System.IO;
using Listen2Me.MVVM.Persistence;
using Listen2Me.MVVM.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Listen2Me.MVVM.System;

/// <inheritdoc/>
public class FileRenamer : IFileRenamer
{
    private readonly ISharedDbContext _dbContext;
    private readonly ILogger _logger;

    public FileRenamer(ISharedDbContext dbContext, ILogger logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <inheritdoc/>
    public bool Rename(Song song)
    {
        var entry = _dbContext.Songs.Entry(song);
        if (entry.State == EntityState.Detached) return true;
        
        var oldPath = entry.Property(s => s.Path).OriginalValue;
        if (string.Equals(oldPath, song.Path, StringComparison.Ordinal)) return false;
        
        try
        {
            File.Move(oldPath, song.Path);
            return true;
        }
        catch (Exception e)
        {
            _logger.Error(e, "Failed to rename file {OldPath} to {NewPath}, rolling back the change", oldPath, song.Path);
            song.SuppressNotifications = true;
            try
            {
                entry.Property(s => s.Path).CurrentValue = oldPath;
            }
            finally
            {
                song.SuppressNotifications = false;
            }
            
            return false;
        }
    }
}