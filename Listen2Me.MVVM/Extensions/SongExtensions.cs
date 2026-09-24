using Listen2Me.MVVM.Persistence.Entities;

namespace Listen2Me.MVVM.Extensions;

public static class SongExtensions
{
    extension(Song song)
    {
        /// <summary>
        /// Maps the song to a dictionary. The dictionary is used to build a filename.
        /// </summary>
        public Dictionary<string, string> MapToDictionary()
        {
            var dict = new Dictionary<string, string>();
            
            void Add(string key, string? value)
            {
                if (!string.IsNullOrEmpty(value))
                    dict[key] = value;
            }
            
            Add("artist", song.Artist);
            Add("title", song.Title);
            Add("genre", song.Genre);
            Add("bpm", song.Bpm != 0 ? song.Bpm.ToString() : null);
            Add("bitrate", song.Bitrate != 0 ? song.Bitrate.ToString() : null);
            
            return dict;
        }
        
        public Song MapFromDictionary(IReadOnlyDictionary<string, string>? dict)
        {
            ArgumentNullException.ThrowIfNull(dict);
            
            dict.TryGetValue("artist", out var artist);
            dict.TryGetValue("title", out var title);
            dict.TryGetValue("genre", out var genre);
            dict.TryGetValue("bpm", out var bpm);
            dict.TryGetValue("bitrate", out var bitrate);
            
            if (!string.IsNullOrEmpty(artist))
                song.Artist = artist;
            
            if (!string.IsNullOrEmpty(title))
                song.Title = title;
            
            if (!string.IsNullOrEmpty(genre))
                song.Genre = genre;
            
            if (!string.IsNullOrEmpty(bpm) && int.TryParse(bpm, out var bpmInt))
                song.Bpm = bpmInt;
            
            if (!string.IsNullOrEmpty(bitrate) && int.TryParse(bitrate, out var bitrateInt))
                song.Bitrate = bitrateInt;
            
            return song;
        }
    }
}