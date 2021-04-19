namespace Listen2Me.Lib.Models
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Song
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public string BPM { get; set; }
        public string Bitrate { get; set; }
        [NotMapped]
        public string Display
        {
            get
            {
                return string.IsNullOrEmpty(Artist) && string.IsNullOrEmpty(Title) ?
                    $"{Path.Split('\\', '/')[^1]}" :
                    $"{Artist} - {Title}";
            }
        }

        public TimeSpan Length { get; set; }
        public string Path { get; set; }
    }
}
