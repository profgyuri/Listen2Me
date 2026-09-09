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
    }
}