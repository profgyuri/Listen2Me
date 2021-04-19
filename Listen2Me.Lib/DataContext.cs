namespace Listen2Me.Lib
{
    using Listen2Me.Lib.Models;

    using System.Data.Entity;

    class DataContext : DbContext
    {
        public DbSet<Song> Songs { get; set; }
    }
}
