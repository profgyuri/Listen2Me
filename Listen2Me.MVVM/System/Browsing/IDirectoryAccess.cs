using System.IO;

namespace Listen2Me.MVVM.System.Browsing;

/// <summary>
/// Contract to make directory access easier to mock.
/// </summary>
public interface IDirectoryAccess
{
    bool Exists(string path);
    string GetDirectoryRoot(string path);
    DriveInfo[] GetDrives();
    IEnumerable<string> EnumerateDirectories(string path);
    DirectoryInfo? GetParent(string path);
}