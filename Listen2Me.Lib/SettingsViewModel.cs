namespace Listen2Me.Lib
{
    using Listen2Me.Lib.Models;
    using Listen2Me.Lib.Utilities;

    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class SettingsViewModel : ViewModelBase
    {
        #region Public Properties
        public ObservableCollection<MusicFolder> MusicFolders { get; set; }
        public MusicFolder SelectedMusicFolder { get; set; }
        public bool ScanOnStartupIsChecked { get; set; }
        #endregion

        #region Commands
        public ICommand RemoveFolderCommand { get; set; }
        public ICommand ScanNowCommand { get; set; }
        #endregion

        public SettingsViewModel()
        {
            DependencyInit();
            DatabaseInitAsync().ConfigureAwait(false);
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
            ScanNowCommand = new RelayCommand(() => throw new System.NotImplementedException());
        }

        private async Task DatabaseInitAsync()
        {
            using DataContext dataContext = new();

            MusicFolders = new ObservableCollection<MusicFolder>(await Task.Run(() => dataContext.MusicFolders).ConfigureAwait(false));
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
