using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using Listen2Me.MVVM.Persistence.Syncing;

namespace Listen2Me.MVVM.Persistence.Entities;

public partial class Song(
    Guid Id,
    string Artist,
    string Title,
    string Genre,
    int Bpm,
    int Bitrate,
    TimeSpan Length,
    string Path,
    long LengthBytes,
    DateTime LastWrite) : ObservableObject, ISyncableEntity
{
    [NotMapped] public string Display => 
        string.IsNullOrWhiteSpace(Artist) || string.IsNullOrWhiteSpace(Title) ? Path : $"{Artist} - {Title}";

    [ObservableProperty] private Guid _id = Id;
    [ObservableProperty] private string _artist = Artist;
    [ObservableProperty] private string _title = Title;
    [ObservableProperty] private string _genre = Genre;
    [ObservableProperty] private int _bpm = Bpm;
    [ObservableProperty] private int _bitrate = Bitrate;
    [ObservableProperty] private TimeSpan _length = Length;
    [ObservableProperty] private string _path = Path;
    [ObservableProperty] private long _lengthBytes = LengthBytes;
    [ObservableProperty] private DateTime _lastWrite = LastWrite;

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