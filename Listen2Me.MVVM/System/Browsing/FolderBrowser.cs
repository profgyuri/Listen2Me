using System.IO;
using Serilog;

namespace Listen2Me.MVVM.System.Browsing;

/// <inheritdoc cref="IFolderBrowser"/>
public class FolderBrowser : IFolderBrowser
{
    private readonly IDirectoryAccess _directory;
    
    private const string ToParentFolder = "..";
    
    public FolderBrowser(IDirectoryAccess directory)
    {
        _directory = directory;
        
        CurrentPath = _directory.GetDirectoryRoot(Environment.CurrentDirectory);
    }
    
    /// <inheritdoc/>
    public string CurrentPath { get; set; }
    
    /// <inheritdoc/>
    public virtual IReadOnlyList<string> GetRoots()
    {
        return _directory.GetDrives()
            .Where(d => d.IsReady)
            .Select(d => d.RootDirectory.FullName)
            .ToList();
    }

    /// <inheritdoc/>
    public virtual async Task<IReadOnlyList<string>> GetSubFolders(string? path = null)
    {
        var target = path ?? CurrentPath;

        return await Task.Run<IReadOnlyList<string>>(() =>
        {
            try
            {
                var result = new List<string>();

                if (!GetRoots().Contains(CurrentPath, StringComparer.OrdinalIgnoreCase))
                {
                    result.Add(ToParentFolder);
                }
                
                var directories = _directory.EnumerateDirectories(target)
                    .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase)
                    .Select(x => new DirectoryInfo(x).Name);
                result.AddRange(directories);
                return result;
            }
            catch (UnauthorizedAccessException)
            {
                return [];
            }
            catch (IOException)
            {
                return [];
            }
        });
    }

    /// <inheritdoc/>
    public void NavigateTo(string path)
    {
        if (!_directory.Exists(path))
            throw new DirectoryNotFoundException($"Cannot navigate to '{path}': directory does not exist.");

        CurrentPath = path;
    }

    /// <inheritdoc/>
    public void NavigateUp()
    {
        var parent = _directory.GetParent(CurrentPath);

        if (parent is not null)
        {
            CurrentPath = parent.FullName;
        }
    }

    /// <inheritdoc/>
    public void NavigateToChild(string name)
    {
        if (name == ToParentFolder)
        {
            NavigateUp();
            return;
        }
        
        var combined = Path.Combine(CurrentPath, name);
        NavigateTo(combined);
    }
}