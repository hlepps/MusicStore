using NAudio.Utils;
using NAudio.Wave;
using NAudioBPM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace MusicStore
{
    public partial class MusicPlayer : Window
    {
        public int songID;
        public string songurlid;

        MediaFoundationReader mf;
        WaveOut waveOut = new WaveOut();
        public MusicPlayer()
        {
            InitializeComponent();
        }

        public void PlaySong(int id)
        {
            songID = id;
            songimage.ImageSource = DB.DBSongsSaved.Get(songID).image.bitmap;
            songurlid = DB.DBSongsSaved.Get(songID).songurlid;
            if (songurlid == "0")
            {
                Close();
                return;
            }
            var url = "https://drive.google.com/uc?id=" + songurlid + "&export=download";
            mf = new MediaFoundationReader(url);
            waveOut.Init(mf);
            waveOut.Play();
            BPMDetector bpm = new BPMDetector(mf, 0, (int)mf.TotalTime.TotalSeconds);
            int interval = (int)((60000f / (bpm.Groups[0].Tempo)) * 1f);
            Trace.WriteLine("BPM: " + bpm.Groups[0].Tempo + "; interval: " + interval);
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, interval);
            dispatcherTimer.Start();
            songSlider.Maximum = mf.TotalTime.TotalSeconds;
            songSlider.TickFrequency = 0.05;
            name.Text = DB.DBSongsSaved.Get(id).authors[0].name + " - " + DB.DBSongsSaved.Get(id).name;
            time.Text = mf.CurrentTime.ToString(@"m\:ss") + " / " + mf.TotalTime.ToString(@"m\:ss");
            this.Show();
        }
        double lastChanged = 0;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            lastChanged = mf.CurrentTime.TotalSeconds;
            songSlider.Value = lastChanged;
            time.Text = mf.CurrentTime.ToString(@"m\:ss") + " / " + mf.TotalTime.ToString(@"m\:ss");
        }
        private void songSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (songSlider.Value != lastChanged)
            {
                mf.CurrentTime = TimeSpan.FromSeconds(songSlider.Value);
                lastChanged = songSlider.Value;
            }
        }
        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            waveOut.Volume = (float)volumeSlider.Value;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        
        }

        private void btnchowanie_click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnzamykanie_click(object sender, RoutedEventArgs e)
        {
            waveOut.Stop();
            Close();
        }

        private void btnpauza_Click(object sender, RoutedEventArgs e)
        {
            switch(waveOut.PlaybackState)
            {
                case PlaybackState.Paused:
                    btnpauza.Content = "⏸";
                    waveOut.Resume();
                    break;

                case PlaybackState.Playing:
                    btnpauza.Content = "⏵";
                    waveOut.Pause();
                    break;

                case PlaybackState.Stopped:
                    btnpauza.Content = "⏸";
                    waveOut.Play();
                    break;
            }
        }

        private void btnstop_Click(object sender, RoutedEventArgs e)
        {
            waveOut.Stop();
            btnpauza.Content = "⏵";
            songSlider.Value = 0;
        }

    }
}
