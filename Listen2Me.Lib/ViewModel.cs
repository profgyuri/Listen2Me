namespace Listen2Me.Lib
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class ViewModel : INotifyPropertyChanged
    {
        #region Interface Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Private Fields

        #endregion

        #region Public Properties

        #endregion

        #region Commands
        public ICommand Stop { get; set; }
        public ICommand PlayPauseCommand { get; set; }
        public ICommand SkipToPrevious { get; set; }
        public ICommand SkipToNext { get; set; }
        public ICommand Shuffle { get; set; }
        #endregion

        private void CommandInit()
        {

        }
    }
}
