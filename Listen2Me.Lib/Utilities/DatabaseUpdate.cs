namespace Listen2Me.Lib.Utilities
{
    using Listen2Me.Lib.Models;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Timers;

    public sealed class DatabaseUpdate : ViewModelBase, IDisposable
    {
        private readonly BackgroundWorker worker;
        private readonly List<string> localPaths;
        private readonly Timer timer;
        private readonly DataContext dataContext;
        private readonly double waitForSeconds = 5;
        private const string defaultLabel = "Scan Now";

        public string Progress { get; set; }
        public bool IsBusy { get; set; }

        public DatabaseUpdate(IEnumerable<string> paths)
        {
            worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

            localPaths = new List<string>(paths);

            timer = new Timer
            {
                Interval = waitForSeconds * 1000,
                AutoReset = false,
                Enabled = false
            };
            timer.Elapsed += TextSetBack;

            dataContext = new();
            dataContext.Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// Resets the text on the scan button 5 seconds after everything is finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextSetBack(object sender, ElapsedEventArgs e)
        {
            Progress = defaultLabel;
            IsBusy = false;
        }

        public void Start()
        {
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
                IsBusy = true;
            }
        }

        public void Cancel()
        {
            if (worker.IsBusy && worker.WorkerSupportsCancellation)
            {
                worker.CancelAsync();
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Progress = "Canceled!";
            }
            else if (e.Error != null)
            {
                Progress = "Error!";
            }
            else
            {
                Progress = "Saving...";

                dataContext.ChangeTracker.DetectChanges();
                dataContext.SaveChanges();

                Progress = "Done!";
            }

            timer.Enabled = true;
            timer.Start();
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = $"{e.ProgressPercentage}%";
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < localPaths.Count; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                AddOrUpdateSong(localPaths[i]);

                worker.ReportProgress((int)((double)i / localPaths.Count * 100));
            }

            DeleteNonExistentEntities();
        }

        private void AddOrUpdateSong(string path)
        {
            Song analyzedSong = SongAnalyzer.Analyze(path);

            if (!dataContext.Songs.Any(song => song.Path == path))
            {
                dataContext.Songs.Add(analyzedSong); //add song if there is no match in database
            }
            else
            {
                UpdateSong(analyzedSong);
            }
        }

        /// <summary>
        /// Updates the entity, where the song's path matches.
        /// </summary>
        /// <param name="song">Entity with the new values.</param>
        private void UpdateSong(Song song)
        {
            Song toUpdate = dataContext.Songs.SingleOrDefault(s => song.SameAs(s)); //update existing song if there is match in database
            song.SongId = toUpdate.SongId;
            toUpdate = song;
        }

        private void DeleteNonExistentEntities()
        {
            IQueryable<Song> toDelete = dataContext.Songs.Where(song => localPaths.Contains(song.Path));
            dataContext.Songs.RemoveRange(toDelete);
        }

        public void Dispose()
        {
            worker.DoWork -= Worker_DoWork;
            worker.ProgressChanged -= Worker_ProgressChanged;
            worker.RunWorkerCompleted -= Worker_RunWorkerCompleted;

            timer.Elapsed -= TextSetBack;

            dataContext.Dispose();
        }
    }
}
