namespace Listen2Me.Lib
{
    using Listen2Me.Lib.Models;

    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class SettingsViewModel : ViewModelBase
    {
        #region Private Fields

        #endregion

        #region Public Properties
        public ObservableCollection<MusicFolder> MusicFolders { get; set; }
        #endregion

        #region Commands
        
        #endregion

        public SettingsViewModel()
        {
            using DataContext dataContext = new();

            foreach (MusicFolder folder in dataContext.MusicFolders)
            {
                MusicFolders.Add(folder);
            }
        }

        #region Initializers
        private void CommandInit()
        {
            
        }
        #endregion
    }
}
