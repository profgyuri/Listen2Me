using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.Tests;

public static class SongFactory
{
    internal static Song CreateTestSongObject()
    {
        return new Song(Guid.Empty, "", "", "", 0, 0, TimeSpan.Zero, "", 0, DateTime.MinValue);
    }
}