namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;

    public static class SongExtensions
    {
        public static bool SameAs(this Song song, Song toCompare)
        {
            return song.Artist == toCompare.Artist && song.Title == toCompare.Title && song.Genre == toCompare.Genre && song.BPM == toCompare.BPM && song.Bitrate == toCompare.Bitrate;
        }
    }
}
