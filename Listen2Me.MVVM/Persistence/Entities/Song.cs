using System.ComponentModel.DataAnnotations.Schema;
using Listen2Me.MVVM.Persistence.Syncing;

namespace Listen2Me.MVVM.Persistence.Entities;

public class Song(
    Guid Id,
    string Artist,
    string Title,
    string Genre,
    int Bpm,
    int Bitrate,
    TimeSpan Length,
    string Path,
    long LengthBytes,
    DateTime LastWrite) : ISyncableEntity
{
    [NotMapped] public string Display => 
        string.IsNullOrWhiteSpace(Artist) || string.IsNullOrWhiteSpace(Title) ? Path : $"{Artist} - {Title}";

    public Guid Id { get; init; } = Id;
    public string Artist { get; set; } = Artist;
    public string Title { get; set; } = Title;
    public string Genre { get; set; } = Genre;
    public int Bpm { get; set; } = Bpm;
    public int Bitrate { get; set; } = Bitrate;
    public TimeSpan Length { get; set; } = Length;
    public string Path { get; init; } = Path;
    public long LengthBytes { get; set; } = LengthBytes;
    public DateTime LastWrite { get; set; } = LastWrite;

    /// <summary>
    /// Maps the properties of the other song to this one.
    /// </summary>
    /// <param name="other">The other song instance to copy the properties from.</param>
    /// <returns>Returns this instance.</returns>
    public Song MapFrom(Song other)
    {
        Artist = other.Artist;
        Title = other.Title;
        Genre = other.Genre;
        Bpm = other.Bpm;
        Bitrate = other.Bitrate;
        Length = other.Length;
        LengthBytes = other.LengthBytes;
        LastWrite = other.LastWrite;

        return this;
    }
}