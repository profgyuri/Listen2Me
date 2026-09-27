namespace Listen2Me.Tests.Persistance.Entities;

[TestClass]
public class SongTests
{
    [ TestMethod]
    public void ChangingFileName_UpdatesPath()
    {
        var originalPath = @"C:\Music\House\Song.mp3";
        var song = SongFactory.CreateTestSongObject();
        song.Path = originalPath;
        
        song.FileName = "NewSong";
        
        Assert.AreEqual(@"C:\Music\House\NewSong.mp3", song.Path);
        Assert.AreEqual("NewSong.mp3", song.FileName);
    }
}