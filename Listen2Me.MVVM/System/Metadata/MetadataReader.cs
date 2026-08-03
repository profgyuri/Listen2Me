using System.IO;
using Listen2Me.MVVM.Persistence.Entities;
using TagLib;

namespace Listen2Me.MVVM.System.Metadata;

/// <inheritdoc/>
public class MetadataReader : IMetadataReader
{
    /// <inheritdoc/>
    public Song Read(string path)
    {
        // PictureLazy mode doesn't load the Properties property.
        using var file = TagLib.File.Create(path, ReadStyle.Average | ReadStyle.PictureLazy);
        var fileInfo = new FileInfo(path);
        
        // Serato's analyzer uses floating point numbers for BPM, which confuses TagLib's parser.
        // This is a workaround to get the correct BPM.
        // example: 150.000 => 150000 gets fixed to 150.
        var bpm = file.Tag.BeatsPerMinute;
        while (bpm > 499)
        {
            bpm /= 10;
        }

        return new Song(
            Id: Guid.NewGuid(),
            Artist: file.Tag.FirstPerformer ?? string.Empty,
            Title: file.Tag.Title ?? string.Empty,
            Genre: file.Tag.FirstGenre ?? string.Empty,
            Bpm: (int)file.Tag.BeatsPerMinute,
            Bitrate: file.Properties.AudioBitrate,
            Length: file.Properties.Duration,
            Path: path,
            LengthBytes: fileInfo.Length,
            LastWrite: fileInfo.LastWriteTime);
    }
}