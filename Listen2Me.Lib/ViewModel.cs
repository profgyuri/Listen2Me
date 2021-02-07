namespace Listen2Me.Lib
{
    using Autofac;

    using Listen2Me.Lib.Models;

    using LowLevelKeyboardHook;

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;
    using System.Timers;

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

        private Timer timer;
        #endregion

        #region Public Properties
        public ObservableCollection<Song> SongList { get; set; }
        public Song SelectedSong { get; set; }
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
            get => playingIndex < SongList?.Count ? SongList[playingIndex].Length.TotalSeconds : 1;
            set
            {
                if (SongList?[playingIndex] is { })
                {
                    SongList[playingIndex].Length = TimeSpan.FromSeconds(value);
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
        }

        #region Initializers
        private void DependencyInit()
        {
            container = IOCContainer.Configure();

            musicPlayer = container.Resolve<IMusicPlayer>();

            keyboardHook = new KeyboardHook(new List<ConsoleKey>() { ConsoleKey.MediaNext, ConsoleKey.MediaPlay, ConsoleKey.MediaPrevious, ConsoleKey.MediaStop });
            keyboardHook.KeyboardPressed += KeyboardHook_KeyboardPressed;
        }

        /// <summary>
        /// Handles the tick event of the <see cref="Timer"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(ElapsedSeconds));
            if (ElapsedSeconds >= TotalSeconds - 1 && musicPlayer.PlaybackState == PlayBackState.Stopped && musicPlayer.HasAudio)
            {
                SkipSong(true);
            }

            if (SongList?.Count > 0)
            {
                IsSkipToNextEnabled = true;
            }
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
                playingIndex = ++playingIndex % SongList.Count;
            }
            else if (playingIndex == 0)
            {
                playingIndex = SongList.Count - 1;
            }
            else
            {
                --playingIndex;
            }

            LoadedSong = SongList[playingIndex];
            SelectedSong = LoadedSong;

            musicPlayer.LoadNewAudio(LoadedSong);
            OnPropertyChanged(nameof(TotalSeconds));
        }
        #endregion
    }
}
