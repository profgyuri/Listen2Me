using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Listen2Me.MVVM.Persistence.Entities;

/// <summary>
/// Represents a music folder.
/// </summary>
public partial class MusicFolder : ObservableObject
{
    /// <summary>
    /// Unique identifier used for database operations.
    /// </summary>
    public required Guid Id { get; init; }
    
    /// <summary>
    /// Full path to the folder.
    /// </summary>
    public required string Path { get; init; }
    
    /// <summary>
    /// Last time the folder was scanned.
    /// </summary>
    public DateTime? LastScan { get; init; }
    
    /// <summary>
    /// Last time the folder was written to on the disk.
    /// </summary>
    public DateTime? LastWrite { get; init; }

    /// <summary>
    /// Whether the folder can be scanned recursively.
    /// </summary>
    [ObservableProperty] private bool _recursivelyScannable;
}