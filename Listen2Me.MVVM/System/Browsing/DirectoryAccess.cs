using System.IO;

namespace Listen2Me.MVVM.System.Browsing;

public class DirectoryAccess : IDirectoryAccess
{
    public virtual bool Exists(string path) => Directory.Exists(path);

    public virtual string GetDirectoryRoot(string path) => Directory.GetDirectoryRoot(path);

    public virtual DriveInfo[] GetDrives() => DriveInfo.GetDrives();

    public virtual IEnumerable<string> EnumerateDirectories(string path) => Directory.EnumerateDirectories(path);

    public virtual DirectoryInfo? GetParent(string path) => Directory.GetParent(path);
}