namespace Listen2Me.UI
{
    using Listen2Me.Lib.Models;
    using Listen2Me.Lib.Utilities;

    using System.Windows;
    using System.Windows.Forms;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddFolderButton_Click(object sender, RoutedEventArgs e)
        {
            MusicFolder newFolder = new();

            using FolderBrowserDialog dialog = new();
            dialog.Description = "Select a folder that contains your music files!";
            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                newFolder.Path = dialog.SelectedPath;

                MusicFolderHelper.AddMusicFolderToDatabase(newFolder);

                (DataContext as Lib.SettingsViewModel).MusicFolders.Add(newFolder);
            }
        }
    }
}
