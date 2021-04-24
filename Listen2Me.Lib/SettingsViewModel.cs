namespace Listen2Me.Lib
{
    using Listen2Me.Lib.Models;
    using Listen2Me.Lib.Utilities;

    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class SettingsViewModel : ViewModelBase
    {
        #region Public Properties
        public ObservableCollection<MusicFolder> MusicFolders { get; set; }
        public MusicFolder SelectedMusicFolder { get; set; }
        #endregion

        #region Commands
        public ICommand RemoveFolderCommand { get; set; }
        #endregion

        public SettingsViewModel()
        {
            DependencyInit();
            DatabaseInit();
            CommandInit();
        }

        #region Initializers
        private void DependencyInit()
        {
            MusicFolders = new ObservableCollection<MusicFolder>();
        }

        private void CommandInit()
        {
            RemoveFolderCommand = new RelayCommand(() => RemoveMusicFolder());
        }

        private void DatabaseInit()
        {
            using DataContext dataContext = new();

            foreach (MusicFolder folder in dataContext.MusicFolders)
            {
                MusicFolders.Add(folder);
            }
        }
        #endregion

        #region Helper Methods
        public void AddMusicFolder(string path)
        {
            MusicFolder newFolder = MusicFolderHelper.AddMusicFolderToDatabase(path);
            MusicFolders.Add(newFolder);
        }

        private void RemoveMusicFolder()
        {
            if (SelectedMusicFolder is not null)
            {
                if (MusicFolderHelper.RemoveFolderFromDatabase(SelectedMusicFolder))
                {
                    MusicFolders.Remove(SelectedMusicFolder);
                }
            }
        }
        #endregion
    }
}
