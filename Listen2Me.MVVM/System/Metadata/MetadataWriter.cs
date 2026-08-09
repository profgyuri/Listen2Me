using System.IO;
using Listen2Me.MVVM.Persistence.Entities;
using Polly;
using Polly.Retry;

namespace Listen2Me.MVVM.System.Metadata;

/// <inheritdoc />
public class MetadataWriter : IMetadataWriter
{
    /// <inheritdoc />
    public async Task UpdateTags(Song song)
    {
        ArgumentNullException.ThrowIfNull(song);

        if (!File.Exists(song.Path))
        {
            throw new FileNotFoundException("Audio file was not found.", song.Path);
        }

        var extension = Path.GetExtension(song.Path);
        if (!Constants.SupportedAudioExtensions.Contains(extension))
        {
            throw new NotSupportedException(
                $"Unsupported format '{extension}'. Supported formats: {string.Join(", ", Constants.SupportedAudioExtensions)}");
        }

        SaveTags(song, extension);
    }

    private void SaveTags(Song song, string extension)
    {
        using var file = TagLib.File.Create(song.Path);
        if (extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
        {
            // Removing, then recreating the RIFF tags is necessary to properly write the metadata.
            file.RemoveTags(TagLib.TagTypes.RiffInfo);
            // Although removing Id3v2 tags does not break the logic,
            // but it may render the tags incompatible with other 3rd party readers.
        }

        var tags = new List<TagLib.Tag> { file.Tag, file.GetTag(TagLib.TagTypes.Id3v2, create: true) };
        if (extension.Equals(".wav", StringComparison.OrdinalIgnoreCase))
        {
            tags.Add(file.GetTag(TagLib.TagTypes.RiffInfo, create: true));
        }

        foreach (var tag in tags)
        {
            ApplyTagUpdate(tag, song);
        }

        file.Save();
    }

    private void ApplyTagUpdate(TagLib.Tag tag, Song song)
    {
        if (!string.IsNullOrWhiteSpace(song.Artist))
        {
            tag.Performers = [song.Artist];

            if (tag is TagLib.Riff.InfoTag riffInfoTag)
            {
                // Some RIFF readers prioritize "IART", others "ISTR". Keep them in sync.
#pragma warning disable CS0618
                riffInfoTag.Artists = [song.Artist];
#pragma warning restore CS0618
            }
        }

        if (!string.IsNullOrWhiteSpace(song.Title))
        {
            tag.Title = song.Title;
        }

        if (!string.IsNullOrWhiteSpace(song.Genre))
        {
            tag.Genres = [song.Genre];
        }

        tag.BeatsPerMinute = (uint)song.Bpm;
    }
    
    private static bool IsSharingViolation(IOException ex)
    {
        const int errorSharingViolation = unchecked((int)0x80070020);
        const int errorLockViolation = unchecked((int)0x80070021);
        return ex.HResult is errorSharingViolation or errorLockViolation;
    }
}