namespace Listen2Me.Lib.Models
{
    using System;
    using NAudio.Wave;

    public sealed class MusicPlayer : IMusicPlayer
    {
        private AudioFileReader audioReader;

        private WaveOutEvent waveOutEvent;

        private bool startSongAutomatically = false;

        public PlayBackState PlaybackState
        {
            get
            {
                return waveOutEvent is null ? PlayBackState.Stopped
                    : (waveOutEvent.PlaybackState switch
                    {
                        NAudio.Wave.PlaybackState.Stopped => PlayBackState.Stopped,
                        NAudio.Wave.PlaybackState.Playing => PlayBackState.Playing,
                        NAudio.Wave.PlaybackState.Paused => PlayBackState.Paused,
                        _ => PlayBackState.Stopped,
                    });
            }
        }
        public bool HasAudio { get; set; }

        public float Volume
        {
            get => audioReader is { } ? audioReader.Volume : 1;
            set
            {
                if (audioReader is { })
                {
                    audioReader.Volume = value;
                }
            }
        }

        public TimeSpan ElapsedTime
        {
            get => audioReader?.CurrentTime ?? TimeSpan.FromSeconds(0);
            set
            {
                if (audioReader is { })
                {
                    audioReader.CurrentTime = value;
                }
            }
        }

        public void LoadNewAudio(Song song)
        {
            Dispose();
            audioReader = new AudioFileReader(song.Path);
            (waveOutEvent ??= new WaveOutEvent()).Init(audioReader);
            HasAudio = true;
            if (startSongAutomatically)
            {
                waveOutEvent?.Play();
            }
        }

        public void Dispose()
        {
            audioReader?.Dispose();
            waveOutEvent?.Dispose();
            HasAudio = false;
        }

        public void Play()
        {
            startSongAutomatically = true;
            if (startSongAutomatically)
            {
                waveOutEvent?.Play();
            }
        }

        public void Pause()
        {
            waveOutEvent?.Pause();
            startSongAutomatically = false;
        }

        public void Stop()
        {
            waveOutEvent?.Stop();
            if (audioReader != null)
            {
                audioReader.CurrentTime = TimeSpan.FromSeconds(0);
            }
            startSongAutomatically = false;
        }
    }
}
