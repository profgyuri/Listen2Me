using System.ComponentModel.DataAnnotations.Schema;
using Listen2Me.MVVM.Persistence.Syncing;

namespace Listen2Me.MVVM.Persistence.Entities;

public record Song(
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
}