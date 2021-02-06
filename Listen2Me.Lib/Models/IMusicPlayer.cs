namespace Listen2Me.Lib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMusicPlayer : IDisposable
    {
        PlayBackState PlaybackState { get; }
        bool HasAudio { get; set; }
        float Volume { get; set; }
        TimeSpan ElapsedTime { get; set; }

        void LoadNewAudio(Song song);
        void Pause();
        void Play();
        void Stop();
    }
}
