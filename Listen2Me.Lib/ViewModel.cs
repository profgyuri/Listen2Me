namespace Listen2Me.Lib
{
    using Autofac;

    using Listen2Me.Lib.Models;
    using Listen2Me.Lib.Utilities;
    using LowLevelKeyboardHook;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Timers;
    using System.Windows.Input;

    using IContainer = Autofac.IContainer;

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
        private IContainer container;

        private IMusicPlayer musicPlayer;

        private KeyboardHook keyboardHook;

        private int playingIndex;
        #endregion

        #region Public Properties
        public ObservableCollection<Song> PlayList { get; set; }
        public Song SelectedPlayListSong { get; set; }
        public ObservableCollection<Song> SearchList { get; set; }
        public Song SelectedSearchListSong { get; set; }
        public Song LoadedSong { get; set; }

        public bool IsSkipToPreviousEnabled { get; set; }
        public bool IsSkipToNextEnabled { get; set; }
        public bool IsShuffleEnabled { get; set; }
        public float Volume
        {
            get => musicPlayer?.Volume ?? 1;
            set => musicPlayer.Volume = value;
        }
        public double ElapsedSeconds
        {
            get => musicPlayer?.ElapsedTime.TotalSeconds ?? 0;
            set
            {
                if (musicPlayer is { })
                {
                    musicPlayer.ElapsedTime = TimeSpan.FromSeconds(value);
                }
            }
        }
        public double TotalSeconds
        {
            get => LoadedSong?.Length.TotalSeconds ?? 1;
            set
            {
                if (PlayList?[playingIndex] is { })
                {
                    PlayList[playingIndex].Length = TimeSpan.FromSeconds(value);
                }
            }
        }

        #endregion

        #region Commands
        public ICommand StopCommand { get; set; }
        public ICommand PlayPauseCommand { get; set; }
        public ICommand SkipToPreviousCommand { get; set; }
        public ICommand SkipToNextCommand { get; set; }
        public ICommand ShuffleCommand { get; set; }
        #endregion

        public ViewModel()
        {
            CommandInit();
            DependencyInit();

            //The sole puprose of the 5 lines below is to help me make the UI.
            LoadedSong = SongAnalyzer.Analyze(@"e:\Zene\Hardstyle\The Prophet & Devin Wild ft. Remi - All In My Head.wav");
            musicPlayer.LoadNewAudio(LoadedSong);
            OnPropertyChanged(nameof(TotalSeconds));

            PlayList.Add(LoadedSong);
            PlayList.Add(LoadedSong);

            Timer timer = new Timer();
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Interval = 100;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(ElapsedSeconds));
        }

        #region Initializers
        private void DependencyInit()
        {
            container = IOCContainer.Configure();

            musicPlayer = container.Resolve<IMusicPlayer>();

            keyboardHook = new KeyboardHook(new List<ConsoleKey>() { ConsoleKey.MediaNext, ConsoleKey.MediaPlay, ConsoleKey.MediaPrevious, ConsoleKey.MediaStop });
            keyboardHook.KeyboardPressed += KeyboardHook_KeyboardPressed;

            PlayList = new ObservableCollection<Song>();
        }

        private void CommandInit()
        {
            PlayPauseCommand = new RelayCommand(() => PlayPause());
            StopCommand = new RelayCommand(() => musicPlayer.Stop());
            SkipToNextCommand = new RelayCommand(() => SkipSong(true));
            SkipToPreviousCommand = new RelayCommand(() => SkipSong(false));
        }
        #endregion

        private void KeyboardHook_KeyboardPressed(object sender, KeyboardHookEventArgs e)
        {
            ConsoleKey key = e.KeyboardData.Key;

            if (e.KeyboardState == Enums.KeyboardState.KeyDown)
            {
                switch (key)
                {
                    case ConsoleKey.MediaNext:
                        SkipSong(true);
                        break;
                    case ConsoleKey.MediaPrevious:
                        SkipSong(false);
                        break;
                    case ConsoleKey.MediaPlay:
                        PlayPause();
                        break;
                    case ConsoleKey.MediaStop:
                        musicPlayer.Stop();
                        break;
                }
            }
        }

        #region Media Controller
        private void PlayPause()
        {
            if (musicPlayer.PlaybackState != PlayBackState.Playing && musicPlayer.HasAudio)
            {
                musicPlayer.Play();
            }
            else if (musicPlayer.HasAudio)
            {
                musicPlayer.Pause();
            }
        }

        /// <summary>
        /// Skips to the next or the previous song.
        /// </summary>
        /// <param name="skipToNext">True to skip to the next song, false to go to the previous song.</param>
        private void SkipSong(bool skipToNext)
        {
            if (skipToNext)
            {
                playingIndex = ++playingIndex % PlayList.Count;
            }
            else if (playingIndex == 0)
            {
                playingIndex = PlayList.Count - 1;
            }
            else
            {
                --playingIndex;
            }

            LoadedSong = PlayList[playingIndex];
            SelectedPlayListSong = LoadedSong;

            musicPlayer.LoadNewAudio(LoadedSong);
            OnPropertyChanged(nameof(TotalSeconds));
        }
        #endregion
    }
}
