namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;

    using System;

    public static class MusicFolderHelper
    {
        public static void AddMusicFolderToDatabase(MusicFolder newFolder)
        {
            using DataContext dataContext = new();
            dataContext.MusicFolders.Add(newFolder);
            dataContext.SaveChanges();
        }

        public static void RemoveFolderFromDatabase(MusicFolder folder)
        {
            using DataContext dataContext = new();

            try
            {
                dataContext.MusicFolders.Remove(folder);
            }
            catch (Exception)
            {
                throw new InvalidOperationException("The specified folder could not be removed from the database! (Maybe it wasn't even there?)");
            }
            finally
            {
                dataContext.SaveChanges();
            }
        }
    }
}
