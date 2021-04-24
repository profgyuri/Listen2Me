namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;

    using System;
    using System.Linq;

    public static class MusicFolderHelper
    {
        public static MusicFolder AddMusicFolderToDatabase(string path)
        {
            using DataContext dataContext = new();
            MusicFolder result = dataContext.MusicFolders.Add(new MusicFolder(){ Path = path });
            dataContext.SaveChanges();

            return result;
        }

        public static bool RemoveFolderFromDatabase(MusicFolder folder)
        {
            bool result = false;

            using DataContext dataContext = new();

            MusicFolder toDelete = dataContext.MusicFolders.Find(folder.MusicFolderId);

            try
            {
                dataContext.MusicFolders.Remove(toDelete);
                result = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The specified folder could not be removed from the database! (Maybe it wasn't even there?) " + ex.Message);
            }
            finally
            {
                dataContext.SaveChanges();
            }

            return result;
        }
    }
}
