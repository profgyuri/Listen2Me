namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    public static class SongAnalyzer
    {
        /// <summary>
        /// Reads the metadata in the given path.
        /// </summary>
        /// <param name="path">Full path to the media file.</param>
        /// <returns></returns>
        public static Song Analyze(string path)
        {
            if (File.Exists(path))
            {
                Song song = new();
                TagLib.File file = TagLib.File.Create(path);
                try
                {
                    song.Artist = string.Join(";", file.Tag.Performers);
                    song.Title = file.Tag.Title;
                    song.Genre = string.Join(";", file.Tag.Genres);
                    song.BPM = file.Tag.BeatsPerMinute + " BPM";
                    song.Bitrate = file.Properties.AudioBitrate + " kbps";
                    song.Length = file.Properties.Duration;
                    song.Path = path;
                }
                catch
                {
                    // I will switch for exception logging later.
                    throw new NotImplementedException();
                }
                finally
                {
                    file.Dispose();
                }

                return song;
            }
            else
            {
                throw new FileNotFoundException($"No file exists on path: {path}");
            }
        }

        public static Task<Song> AnalyzeAsync(string path)
        {
            return Task.Run(() => Analyze(path));
        }

        public static IEnumerable<Song> Analyze(string[] paths)
        {
            foreach (string path in paths)
            {
                yield return Analyze(path);
            }
        }

        public static async IAsyncEnumerable<Song> AnalyzeAsync(string[] paths)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                yield return await Task.Run(() => Analyze(paths[i])).ConfigureAwait(false);
            }
        }
    }
}
