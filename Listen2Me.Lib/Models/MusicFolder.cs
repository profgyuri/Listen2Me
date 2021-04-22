namespace Listen2Me.Lib.Models
{
    using System.ComponentModel.DataAnnotations;

    public class MusicFolder
    {
        public int MusicFolderId { get; set; }
        [MaxLength(500)]
        public string Path { get; set; }
    }
}
