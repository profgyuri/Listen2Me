namespace Listen2Me.Lib.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Song
    {
        public int SongId { get; set; }
        [MaxLength(200)]
        public string Artist { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(50)]
        public string Genre { get; set; }
        [MaxLength(4)]
        public string BPM { get; set; }
        [MaxLength(4)]
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
        [Required]
        public TimeSpan Length { get; set; }
        [Required]
        [MaxLength(500)]
        public string Path { get; set; }
    }
}
