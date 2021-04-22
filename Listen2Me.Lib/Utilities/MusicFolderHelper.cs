namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;

    public static class MusicFolderHelper
    {
        public static void AddMusicFolderToDatabase(MusicFolder newFolder)
        {
            using DataContext dataContext = new();
            dataContext.MusicFolders.Add(newFolder);
            dataContext.SaveChanges();
        }
    }
}
