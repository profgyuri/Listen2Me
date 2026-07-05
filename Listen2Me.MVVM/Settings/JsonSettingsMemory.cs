using System.IO;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Listen2Me.MVVM.Settings;

/// <inheritdoc cref="ISettingsMemory"/>
public abstract class JsonSettingsMemory : ObservableObject, ISettingsMemory
{
    /// <summary>
    /// Setting-category-specific file name.
    /// </summary>
    protected abstract string FileName { get; }
    
    private readonly SemaphoreSlim _saveLock = new(1, 1);
    private readonly SemaphoreSlim _loadLock = new(1, 1);
    
    /// <inheritdoc/>
    public async Task SaveAsync(CancellationToken ct = default)
    {
        await _saveLock.WaitAsync(ct);
        try
        {
            var path = Path.Combine(Constants.SettingsFolder, FileName);
            Directory.CreateDirectory(Constants.SettingsFolder);

            await using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(fs, this, GetType(), cancellationToken: ct);
        }
        finally
        {
            _saveLock.Release();
        }
    }

    /// <inheritdoc/>
    public async Task LoadAsync(CancellationToken ct = default)
    {
        await _loadLock.WaitAsync(ct);
        try
        {
            var path = Path.Combine(Constants.SettingsFolder, FileName);

            if (!File.Exists(path))
            {
                return;
            }
            
            await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            var deserialized = await JsonSerializer.DeserializeAsync(fs, GetType(), cancellationToken: ct);

            if (deserialized is null)
                return;

            foreach (var property in GetType().GetProperties().Where(p => p.CanWrite))
            {
                property.SetValue(this, property.GetValue(deserialized));
            }
        }
        finally
        {
            _loadLock.Release();
        }
    }
}