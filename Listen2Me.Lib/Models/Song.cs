namespace Listen2Me.Lib.Models
{
    using System;

    public class Song
    {
        public int Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }

        public string Display
        {
            get
            {
                return !string.IsNullOrEmpty(Artist) && !string.IsNullOrEmpty(Title) ?
                    $"{Artist} - {Title}" :
                    !string.IsNullOrEmpty(Title) ? $"{Title}" :
                    $"{Path.Split('\\', '/')[^1]}";
            }
        }

        public TimeSpan Length { get; set; }
        public string Path { get; set; }
    }
}
