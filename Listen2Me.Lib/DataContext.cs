namespace Listen2Me.Lib
{
    using Listen2Me.Lib.Models;

    using System.Data.Entity;

    public class DataContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<MusicFolder> MusicFolders { get; set; }
    }
}
